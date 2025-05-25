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
    }

}
