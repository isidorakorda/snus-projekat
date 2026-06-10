using Microsoft.EntityFrameworkCore;
using SensorRegistry.Models;

namespace SensorRegistry.Data
{
    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }

        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorRecord> SensorRecords { get; set; }
    }
}
