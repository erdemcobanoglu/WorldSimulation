using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MapController : Controller
{
    [HttpGet("generate")]
    public IActionResult Generate()
    {
        return Ok(new { message = "Harita oluşturuldu" });
    }
}
