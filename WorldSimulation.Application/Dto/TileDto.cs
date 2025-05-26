using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class TileDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Terrain { get; set; }
        public string Weather { get; set; }
        public string OceanEvent { get; set; }
    }
}
