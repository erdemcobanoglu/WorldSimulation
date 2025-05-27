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
            var weatherOptions = Enum.GetValues(typeof(WeatherType));
            var oceanEvents = Enum.GetValues(typeof(OceanEventType));
            var rand = new Random();

            foreach (var tile in map.Tiles)
            {
                // Yeni güncellenen çift alanlar
                tile.CurrentWeather = (WeatherType)weatherOptions.GetValue(rand.Next(weatherOptions.Length));
                tile.CurrentWeather = (WeatherType)weatherOptions.GetValue(rand.Next(weatherOptions.Length));

                if (tile.Terrain == TerrainType.Sea)
                {
                    var randomOceanEvent = rand.NextDouble() < 0.5
                        ? (OceanEventType?)oceanEvents.GetValue(rand.Next(oceanEvents.Length))
                        : null;

                    tile.CurrentOceanEvent = randomOceanEvent;
                    tile.CurrentOceanEvent = randomOceanEvent;
                }
                else
                {
                    tile.CurrentOceanEvent = null;
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
