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
    public class SimulationEngine : ISimulationEngine
    {
        private readonly IWeatherService _weatherService;

        public SimulationEngine(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public List<SimulationTickResult> Run(WorldMap map, int maxTicks = 100)
        {
            var results = new List<SimulationTickResult>();

            for (int tick = 0; tick < maxTicks; tick++)
            {
                WeatherType weather = _weatherService.GetCurrentWeather(tick);

                var tickResult = new SimulationTickResult
                {
                    Tick = tick,
                    Weather = weather.ToString(),
                    WeatherSymbol = GetWeatherSymbol(weather),
                    MapLines = RenderMap(map)
                };

                results.Add(tickResult);
            }

            return results;
        }

        private List<string> RenderMap(WorldMap map)
        {
            var lines = new List<string>();

            for (int y = 0; y < map.Height; y++)
            {
                var line = new StringBuilder();

                for (int x = 0; x < map.Width; x++)
                {
                    Tile tile = map.Tiles[x, y];
                    string symbol = tile.Terrain switch
                    {
                        TerrainType.Land => "🟫",
                        TerrainType.Sea => "🌊",
                        TerrainType.Air => "☁️",
                        _ => "?"
                    };
                    line.Append(symbol);
                }

                lines.Add(line.ToString());
            }

            return lines;
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
                        TerrainType.Land => "🟫",
                        TerrainType.Sea => "🌊",
                        TerrainType.Air => "☁️",
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
