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

        public void Run(WorldMap map, int maxTicks)
        {
            int tick = 0;

            while (tick < maxTicks)
            {
                Console.Clear();
                var currentTime = DateTime.Now;

                // ☁️ Hava durumu güncelle
                _weatherService.UpdateWeather(map, currentTime);

                // 🌊 Okyanus olaylarını güncelle
                _oceanEventService?.Update(currentTime);

                // 🖨 Haritayı yazdır
                PrintMap(map);

                // 🎯 Okyanus olaylarını tek satırda yaz
                if (_oceanEventService != null)
                {
                    // Konumu ayarla (örneğin en alt satıra yakın bir yere)
                    int line = Console.CursorTop;
                    Console.SetCursorPosition(0, line);

                    var activeEvents = _oceanEventService.GetActiveEvents();

                    string lineText = "Olaylar: ";
                    foreach (var evt in activeEvents)
                    {
                        lineText += $"[{evt.EventType} @({evt.Location.X},{evt.Location.Y}) I:{evt.Intensity:F1}] ";
                    }

                    // Satırı temizle (görsel çakışmayı önlemek için)
                    Console.Write(lineText.PadRight(Console.WindowWidth));
                }

                tick++;
                Thread.Sleep(1000);
            }
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
