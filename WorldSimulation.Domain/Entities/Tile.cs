using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Domain.Entities
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TerrainType Terrain { get; set; }
        public OceanEventType? CurrentOceanEvent { get; set; }
        public WeatherType CurrentWeather { get; set; } = WeatherType.Sunny;

    }
}
