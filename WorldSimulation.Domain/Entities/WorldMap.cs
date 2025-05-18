using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Domain.Entities
{
    public class WorldMap
    {
        public int Width { get; }
        public int Height { get; }
        public Tile[,] Tiles { get; }

        public WorldMap(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            GenerateRandomTerrain();
        }

        private void GenerateRandomTerrain()
        {
            var random = new Random();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new Tile
                    {
                        X = x,
                        Y = y,
                        Terrain = (TerrainType)random.Next(0, 3)
                    };
                }
            }
        }
    }
}
