using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "pong", time = DateTime.UtcNow });
    }

}