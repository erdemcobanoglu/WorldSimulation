using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Domain.Interfaces
{
    public interface IWorldEvent
    {
        string EventType { get; }
        int X { get; }
        int Y { get; }
        double Intensity { get; }
        int Duration { get; }
        DateTime StartTime { get; }
    }
}
