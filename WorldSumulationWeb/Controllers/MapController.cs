using Microsoft.AspNetCore.Mvc;
using WorldSimulation.Application.Interfaces;

namespace WorldSumulation.Web.Controllers
{
    public class MapController : ControllerBase
    {
        private readonly IWorldMapService _mapService;

        public MapController(IWorldMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpGet("generate")]
        public IActionResult Generate()
        {
            var map = _mapService.CreateMap(30, 10);
            return Ok(map); // JSON olarak döner
        }
    }
}
