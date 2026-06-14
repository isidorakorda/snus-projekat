using ConsensusService.Data;
using ConsensusService.Enum;
using Microsoft.EntityFrameworkCore;
using ConsensusService.Model;

namespace ConsensusService
{
    public class ConsensusWorker : BackgroundService
    {
        private readonly ILogger<ConsensusWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsensusWorker(ILogger<ConsensusWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ConsensusDbContext>();

                    var now = DateTime.UtcNow;
                    var from = now.AddMinutes(-1);

                    var records = await db.SensorRecords
                        .AsNoTracking()
                        .Join(db.Sensors.AsNoTracking(),
                            r => r.SensorId,
                            s => s.Id,
                            (r, s) => new { r, s })
                        .Where(x => 
                            x.r.Timestamp >= from &&
                            x.r.Timestamp < now &&
                            (x.s.Quality == DataQuality.GOOD || x.s.Quality == DataQuality.UNCERTAIN))
                        .Select(x => new
                        {
                            x.r.SensorId,
                            x.r.Temperature
                        })
                        .ToListAsync(stoppingToken);
                    if (records == null || records.Count < 4)
                    {
                        _logger.LogInformation("No Data To Calculate Consensus With");
                        continue;
                    }
                    var sorted = records.Select(x => x.Temperature).OrderBy(x => x).ToList();
                    var consensusValue = sorted[sorted.Count / 2];

                    Consensus consensus = new Consensus
                    {
                        Id = Guid.NewGuid(),
                        Temperature = consensusValue,
                        Timestamp = DateTime.UtcNow,
                    };

                    await db.ConsensusValues.AddAsync(consensus, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("[CONSENSUS] Consensus reached: {Temperature} degrees", consensus.Temperature);

                    double q1 = CalculatePercentile(sorted, 0.25);
                    double q3 = CalculatePercentile(sorted, 0.75);
                    double iqr = q3 - q1;

                    double lowerBound = q1 - 1.5 * iqr;
                    double upperBound = q3 + 1.5 * iqr;

                    var outlierCandidates = records
                        .GroupBy(x => x.SensorId)
                        .Select(g =>
                        {
                            var totalCount = g.Count();
                            var outlierCount = g.Count(x => x.Temperature < lowerBound || x.Temperature > upperBound);
                            var outlierPercentage = totalCount == 0 ? 0 : (double)outlierCount * 100.00 / totalCount;
                            return new
                            {
                                SensorId = g.Key,
                                OutlierPercentage = outlierPercentage
                            };
                        })
                        .Where(s => s.OutlierPercentage > 80.00)
                        .Select(y => y.SensorId)
                        .ToList();

                    var sensors = await db.Sensors
                        .Where(s => outlierCandidates.Contains(s.Id))
                        .ToListAsync(stoppingToken);

                    foreach (var sensor in sensors)
                        sensor.Quality = sensor.Quality == DataQuality.UNCERTAIN ? DataQuality.BAD : DataQuality.UNCERTAIN;

                    await db.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("[MALICIOUS NODES] Found and labeled {Count} possible malicious nodes", outlierCandidates.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error happened while trying to calculate consensus");
                }
            }
        }

        public static double CalculatePercentile(List<double> sortedValues, double percentile)
        {
            double position = (sortedValues.Count - 1) * percentile;
            int left = (int)Math.Floor(position);
            int right = (int)Math.Ceiling(position);

            if (left == right)
                return sortedValues[left];

            double fraction = position - left;
            return sortedValues[left] + (sortedValues[right] - sortedValues[left]) * fraction;
        }
    }
}

