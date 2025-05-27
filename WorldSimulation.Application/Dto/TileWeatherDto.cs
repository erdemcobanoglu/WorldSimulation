using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class TileWeatherDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Weather { get; set; }
        public string Terrain { get; set; } // ← bu olmalı!
        public string? OceanEvent { get; set; }
    }
}
