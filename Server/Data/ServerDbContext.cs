using Microsoft.EntityFrameworkCore;
using Server.Model;


namespace Server.Data
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options) { }

        public DbSet<SensorRecord> SensorRecords { get; set; }
    }
}
