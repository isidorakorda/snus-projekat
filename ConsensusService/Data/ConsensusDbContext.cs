using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsensusService.Model;

namespace ConsensusService.Data
{
    public class ConsensusDbContext : DbContext
    {
        public ConsensusDbContext(DbContextOptions<ConsensusDbContext> options) : base(options) { }
        
        public DbSet<SensorRecord> SensorRecords { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Consensus> ConsensusValues { get; set; }
    }
}
