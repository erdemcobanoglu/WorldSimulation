using Microsoft.AspNetCore.Mvc;
using WorldSimulation.Application.Interfaces;
using WorldSimulation.Application.Service;
using WorldSimulation.Application.WorldMapService;
using WorldSimulation.Domain.Entities;
using WorldSimulation.Domain.Enums;

namespace WorldSimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapController : ControllerBase
    {
        private readonly IWorldMapService _mapService;
        private readonly IWeatherService _weatherService;

        public MapController(IWorldMapService mapService, IWeatherService weatherService)
        {
            _mapService = mapService;
            _weatherService = weatherService;
        }

        // https://localhost:7260/api/map/generate
        [HttpGet("generate")]
        public IActionResult Generate()
        {
            var map = _mapService.CreateMap(30, 10);

            // Bu servisler parametreye bağlı olduğu için new ile oluşturuluyor
            var oceanEventService = new OceanEventService(map);
            var simulation = new WeatherSimulationEngine(_weatherService, oceanEventService);

            simulation.Run(map,100);

            return Ok(map);  
        }

        // https://localhost:7260/api/map/generatev2
        [HttpGet("generatev2")]
        public IActionResult Generatev2()
        { 

            // Örnek rastgele veri üretimi
            var random = new Random();

            var data = new
            {
                id = Guid.NewGuid(),
                timestamp = DateTime.UtcNow,
                temperature = random.Next(-20, 40), // -20°C ile 40°C arası  
                weather = GetRandomWeather(),
                city = GetRandomCity()
            };

            return Ok(data);
        }

        // Yardımcı metodlar
        private string GetRandomWeather()
        {
            string[] conditions = { "Güneşli", "Yağmurlu", "Bulutlu", "Fırtınalı", "Karl", "Sisli" };
            return conditions[new Random().Next(conditions.Length)];
        }

        private string GetRandomCity()
        {
            string[] cities = { "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya", "Trabzon" };
            return cities[new Random().Next(cities.Length)];
        }
    }

}
