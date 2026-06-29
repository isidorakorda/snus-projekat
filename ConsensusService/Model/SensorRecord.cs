using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusService.Model
{
    public class SensorRecord
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public double Temperature { get; set; }
        public int MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public int AlarmPriority { get; set; }
    }
}
