using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;

namespace WorldSimulation.Infrastructure.Providers
{
    public class MapProvider : IMapProvider
    {
        private WorldMap _map;

        public WorldMap GetMap()
        {
            if (_map == null)
                throw new InvalidOperationException("Map has not been initialized.");

            return _map;
        }

        public void SetMap(WorldMap map)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public bool HasMap() => _map != null;

        public void ResetMap()
        {
            _map = null;
        }
    }
}
