using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Application.Service
{
    public class WeatherSimulationEngine : IWeatherSimulationEngine
    {
        private readonly IWeatherService _weatherService; 
        private readonly IOceanEventService _oceanEventService;
         
        public WeatherSimulationEngine(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
         
        public WeatherSimulationEngine(IWeatherService weatherService, IOceanEventService oceanEventService)
        {
            _weatherService = weatherService;
            _oceanEventService = oceanEventService;
        }

        public void Run(WorldMap map, int maxTicks = 100)
        {
            while (true)
            {
                var currentTime = DateTime.Now;

                // Hava durumu güncelle
                _weatherService.UpdateWeather(map, currentTime);

                // 🌊 Okyanus olaylarını güncelle
                _oceanEventService.Update(currentTime);

                // Olayları ekrana basmak istersen:
                foreach (var evt in _oceanEventService.GetActiveEvents())
                {
                    Console.WriteLine($"[🌊 {evt.EventType}] at ({evt.Location.X},{evt.Location.Y}) | Intensity: {evt.Intensity:F1}");
                }

                Thread.Sleep(1000);
            }
        }

        private void PrintMap(WorldMap map)
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Tile tile = map.Tiles[x, y];
                    string symbol = tile.Terrain switch
                    {
                        TerrainType.Land => "L",
                        TerrainType.Sea => "S",
                        TerrainType.Air => "A",
                        _ => "?"
                    };
                    Console.Write(symbol);
                }
                Console.WriteLine();
            }
        }

        private string GetWeatherSymbol(WeatherType weather)
        {
            return weather switch
            {
                WeatherType.Sunny => "🌞",
                WeatherType.Cloudy => "☁️",
                WeatherType.Rainy => "🌧",
                WeatherType.Stormy => "🌩",
                WeatherType.Snowy => "❄️",
                _ => "?"
            };
        }
    }
}
