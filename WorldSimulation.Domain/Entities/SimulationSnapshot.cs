using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Entities.Event;

namespace WorldSimulation.Domain.Entities
{
    public class SimulationSnapshot
    {
        public DateTime Time { get; set; }
        public List<string> Atmosphere { get; set; }
        public List<string> Ocean { get; set; }
        public List<string> Surface { get; set; }
        public List<OceanEvent> ActiveEvents { get; set; }
    }
}
