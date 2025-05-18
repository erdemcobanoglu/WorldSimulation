using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;

namespace WorldSimulation.Application.WorldMapService
{
    public class WorldMapService : IWorldMapService
    {
        public WorldMap CreateMap(int width, int height)
        {
            return new WorldMap(width, height);
        }
    }
}
