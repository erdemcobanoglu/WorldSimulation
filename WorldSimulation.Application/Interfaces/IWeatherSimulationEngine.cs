using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Dto;
using WorldSimulation.Domain.Entities;

namespace WorldSimulation.Application.Interfaces
{ 
    public interface IWeatherSimulationEngine
    {
        List<SimulationSnapshotDto> Run(WorldMap map, int maxTicks = 100);

    }
}
