using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Dto;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Service
{
    public class WeatherService : IWeatherService
    {
        public WeatherType GetCurrentWeather(int tick)
        {
            // Döngüsel olarak hava durumu değişimi örneği (her 10 tick'te değişir)
            int phase = tick % 50;

            return phase switch
            {
                <= 10 => WeatherType.Sunny,
                <= 20 => WeatherType.Cloudy,
                <= 30 => WeatherType.Rainy,
                <= 40 => WeatherType.Stormy,
                _ => WeatherType.Snowy
            };
        }

        public void UpdateWeather(WorldMap map, DateTime currentTime)
        {
            var rand = new Random();

            foreach (var tile in map.Tiles)
            {
                WeatherType weather;

                switch (tile.Terrain)
                {
                    case TerrainType.Desert:
                        weather = rand.NextDouble() < 0.8 ? WeatherType.Sunny : WeatherType.Cloudy;
                        break;

                    case TerrainType.Mountain:
                        weather = rand.NextDouble() < 0.5 ? WeatherType.Snowy : WeatherType.Stormy;
                        break;

                    case TerrainType.Ice:
                        weather = WeatherType.Snowy;
                        break;

                    case TerrainType.Sea:
                        weather = rand.NextDouble() < 0.5 ? WeatherType.Stormy : WeatherType.Rainy;
                        break;

                    case TerrainType.Island:
                        weather = rand.NextDouble() < 0.4 ? WeatherType.Sunny : WeatherType.Rainy;
                        break;

                    default: // Land, Air vs.
                        weather = (WeatherType)rand.Next(Enum.GetValues(typeof(WeatherType)).Length);
                        break;
                }

                tile.CurrentWeather = weather; 

                // Ocean events sadece Sea için
                if (tile.Terrain == TerrainType.Sea)
                {
                    tile.CurrentOceanEvent = rand.NextDouble() < 0.4
                        ? (OceanEventType?)rand.Next(Enum.GetValues(typeof(OceanEventType)).Length)
                        : null;
                }
                else
                {
                    tile.CurrentOceanEvent = null;
                }
            }
        }


        public WeatherSnapshotDto GetWeatherSnapshot(WorldMap map, DateTime time)
        {
            var tiles = new List<TileWeatherDto>();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var tile = map.Tiles[x, y];

                    tiles.Add(new TileWeatherDto
                    {
                        X = tile.X,
                        Y = tile.Y,
                        Weather = tile.CurrentWeather.ToString(),
                        Terrain = tile.Terrain.ToString(),
                    });
                }
            }

            return new WeatherSnapshotDto
            {
                Time = time,
                Tiles = tiles
            };
        }

    }
}
