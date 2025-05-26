using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Interfaces;

namespace WorldSimulation.Domain.Entities.Event
{
    public class EarthquakeEvent : IWorldEvent
    {
        public Tile Epicenter { get; set; }
        public int Duration { get; set; }
        public double Magnitude { get; set; }
        public DateTime StartTime { get; set; }

        public string EventType => "Earthquake";
        public int X => Epicenter.X;
        public int Y => Epicenter.Y;
        public double Intensity => Magnitude; // Mapping
    }
}
