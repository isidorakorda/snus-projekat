using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsensusService.Model
{
    public class Consensus
    {
        public Guid Id { get; set; }
        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
