using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation.Application.Dto
{
    public class MapDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<TileDto> Tiles { get; set; }
    }
}
