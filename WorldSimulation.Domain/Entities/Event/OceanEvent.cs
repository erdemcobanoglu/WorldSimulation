using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Enums;
using WorldSimulation.Application.Interfaces;

namespace WorldSimulation.Domain.Entities.Event
{
    public class OceanEvent : IWorldEvent
    {
        public OceanEventType OceanEventType { get; set; }
        public Tile Location { get; set; }
        public int Duration { get; set; }
        public double Intensity { get; set; }
        public DateTime StartTime { get; set; }

        public string EventType => OceanEventType.ToString();
        public int X => Location.X;
        public int Y => Location.Y;
    }
}
