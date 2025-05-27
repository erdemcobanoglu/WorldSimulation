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
            GenerateTerrainWithNoise();
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

        private void GenerateTerrainLikeWorld()
        {
            var rand = new Random();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Başlangıç değeri olarak "Sea" veriyoruz
                    TerrainType terrain = TerrainType.Sea;

                    // Ortaya kıta bölgesi yerleştir
                    if ((x >= Width / 4 && x <= 3 * Width / 4) &&
                        (y >= Height / 4 && y <= 3 * Height / 4))
                    {
                        // %80 ihtimalle Land, %20 Sea/Air gibi varyasyon
                        double noise = rand.NextDouble();
                        if (noise < 0.8)
                            terrain = TerrainType.Land;
                        else
                            terrain = rand.Next(2) == 0 ? TerrainType.Sea : TerrainType.Air;
                    }

                    // Uçlara doğru Sea baskın
                    if (x < Width / 6 || x > 5 * Width / 6)
                    {
                        terrain = TerrainType.Sea;
                    }

                    Tiles[x, y] = new Tile
                    {
                        X = x,
                        Y = y,
                        Terrain = terrain
                    };
                }
            }
        }

        private void GenerateTerrainWithNoise()
        {
            var rand = new Random();
            double scale = 0.1; // daha yumuşak geçiş için düşük değer
            double islandThreshold = 0.55;
            double landThreshold = 0.45;
            double mountainThreshold = 0.75;
            double desertThreshold = 0.6;
            double iceThreshold = 0.2;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double nx = (double)x / Width - 0.5;
                    double ny = (double)y / Height - 0.5;

                    // Basit noise fonksiyonu (Perlin yerine)
                    double elevation = Math.Sin(5 * nx) * Math.Cos(5 * ny) + rand.NextDouble() * 0.2;

                    TerrainType terrain;

                    if (elevation > mountainThreshold)
                        terrain = TerrainType.Mountain;
                    else if (elevation > desertThreshold)
                        terrain = TerrainType.Desert;
                    else if (elevation > islandThreshold)
                        terrain = TerrainType.Island;
                    else if (elevation > landThreshold)
                        terrain = TerrainType.Land;
                    else if (elevation < iceThreshold)
                        terrain = TerrainType.Ice;
                    else
                        terrain = TerrainType.Sea;

                    Tiles[x, y] = new Tile
                    {
                        X = x,
                        Y = y,
                        Terrain = terrain
                    };
                }
            }
        }

    }
}
