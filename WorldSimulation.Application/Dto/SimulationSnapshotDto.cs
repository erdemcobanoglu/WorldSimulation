using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class SimulationSnapshotDto
    {
        public DateTime Time { get; set; }
        public List<string> Atmosphere { get; set; }
        public List<string> Ocean { get; set; }
        public List<string> Surface { get; set; }
        public List<OceanEventDto> ActiveEvents { get; set; }
    }
}
