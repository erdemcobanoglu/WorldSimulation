using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Domain.Entities
{
    public class OceanEvent
    {
        public OceanEventType EventType { get; set; }
        public Tile Location { get; set; }
        public int Duration { get; set; } // Süre, simülasyon adımı cinsinden
        public double Intensity { get; set; } // Etki şiddeti
        public DateTime StartTime { get; set; }
    }
}
