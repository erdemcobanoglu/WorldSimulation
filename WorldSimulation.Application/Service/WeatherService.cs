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
            int tick = (int)(currentTime - DateTime.Today).TotalSeconds;

            WeatherType currentWeather = GetCurrentWeather(tick);

            // Haritadaki tüm tile'lara hava durumu uygula
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    map.Tiles[x, y].CurrentWeather = currentWeather;
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
