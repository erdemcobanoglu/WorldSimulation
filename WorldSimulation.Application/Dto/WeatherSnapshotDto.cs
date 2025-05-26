using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class WeatherSnapshotDto
    {
        public DateTime Time { get; set; }
        public List<TileWeatherDto> Tiles { get; set; }
    }
}
