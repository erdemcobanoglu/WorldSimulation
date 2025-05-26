using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Entities.Event;
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

        public List<SimulationSnapshot> Run(WorldMap map, int maxTicks)
        {
            var snapshots = new List<SimulationSnapshot>();

            for (int tick = 0; tick < maxTicks; tick++)
            {
                var currentTime = DateTime.Now;

                _weatherService.UpdateWeather(map, currentTime);
                _oceanEventService?.Update(currentTime);

                var snapshot = CreateSnapshot(map, currentTime);
                snapshots.Add(snapshot);

                Thread.Sleep(1000);
            }

            return snapshots;
        }

        private SimulationSnapshot CreateSnapshot(WorldMap map, DateTime currentTime)
        {
            var y = 0;

            var atmosphere = new List<string>();
            var ocean = new List<string>();
            var surface = new List<string>();

            for (int x = 0; x < map.Width; x++)
            {
                var tile = map.Tiles[x, y];

                // Atmosfer
                atmosphere.Add(GetWeatherSymbol(tile.CurrentWeather));

                // Okyanus
                if (tile.Terrain == TerrainType.Sea)
                {
                    ocean.Add(tile.CurrentOceanEvent != null ? "🌊" : "~");
                }
                else
                {
                    ocean.Add(" ");
                }

                // Yüzey
                surface.Add(tile.Terrain switch
                {
                    TerrainType.Land => "L",
                    TerrainType.Sea => "S",
                    TerrainType.Air => "A",
                    _ => "?"
                });
            }

            var activeEvents = _oceanEventService?.GetActiveEvents()
                .Select(ev => new OceanEvent
                {
                    EventType = ev.EventType,
                    Y = ev.Location.X,
                    X = ev.Location.Y,
                    Intensity = ev.Intensity,
                    Duration = ev.Duration,
                    StartTime = ev.StartTime
                }).ToList() ?? new List<OceanEvent>();

            return new SimulationSnapshot
            {
                Time = currentTime,
                Atmosphere = atmosphere,
                Ocean = ocean,
                Surface = surface,
                ActiveEvents = activeEvents
            };
        }


        private void PrintMap(WorldMap map)
        {
            int y = 0; // sadece ilk satır çizilecek

            // 🟦 Atmosfer
            Console.Write("Atmosfer:  ");
            for (int x = 0; x < map.Width; x++)
            {
                Tile tile = map.Tiles[x, y];
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(GetWeatherSymbol(tile.CurrentWeather));
            }
            Console.WriteLine();

            // 🌊 Okyanus
            Console.Write("Okyanus:   ");
            for (int x = 0; x < map.Width; x++)
            {
                Tile tile = map.Tiles[x, y];

                if (tile.Terrain == TerrainType.Sea)
                {
                    if (tile.CurrentOceanEvent != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("🌊"); // olay varsa göster
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("~"); // deniz ama olay yoksa simge
                    }
                }
                else
                {
                    Console.Write(" "); // deniz değilse boşluk bırak
                }
            }
            Console.WriteLine();


            // 🌍 Yüzey
            Console.Write("Yüzey:     ");
            for (int x = 0; x < map.Width; x++)
            {
                Tile tile = map.Tiles[x, y];

                Console.ForegroundColor = tile.Terrain switch
                {
                    TerrainType.Land => ConsoleColor.Green,
                    TerrainType.Sea => ConsoleColor.Blue,
                    TerrainType.Air => ConsoleColor.White,
                    _ => ConsoleColor.Gray
                };

                string symbol = tile.Terrain switch
                {
                    TerrainType.Land => "L",
                    TerrainType.Sea => "S",
                    TerrainType.Air => "A",
                    _ => "?"
                };

                Console.Write(symbol);
            }

            Console.ResetColor();
            Console.WriteLine("\n");
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
