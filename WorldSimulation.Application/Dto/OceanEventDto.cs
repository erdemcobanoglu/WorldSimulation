using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class OceanEventDto
    {
        public string EventType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Intensity { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
    }
}
