using WorldSimulation.Application.Interfaces;
using WorldSimulation.Application.Service; 
using WorldSimulation.Application.WorldMapService;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Harita oluştur
IWorldMapService mapService = new WorldMapService();
WorldMap map = mapService.CreateMap(30, 10);

// Hava ve simülasyon servisleri
IWeatherService weatherService = new WeatherService();
IWeatherSimulationEngine simulation = new WeatherSimulationEngine(weatherService);

// Simülasyonu başlat
simulation.Run(map);
