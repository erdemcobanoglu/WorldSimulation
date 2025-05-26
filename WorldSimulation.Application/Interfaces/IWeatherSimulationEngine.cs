using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Entities;

namespace WorldSimulation.Application.Interfaces
{ 
    public interface IWeatherSimulationEngine
    {
        List<SimulationSnapshot> Run(WorldMap map, int maxTicks = 100);

    }
}
