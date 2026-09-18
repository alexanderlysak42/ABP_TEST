using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Controllers;

[ApiController]
[Route("api/halls")]
public class HallsController : ControllerBase
{
    private readonly IHallService _hallService;
    
    public HallsController(IHallService hallService)
    {
        _hallService = hallService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<HallResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HallResponseDto>>> GetAllAsync()
    {
        return Ok(await _hallService.GetAllAsync());
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallResponseDto>> GetByIdAsync(int id)
    {
        return Ok(await _hallService.GetByIdAsync(id));
    }
}