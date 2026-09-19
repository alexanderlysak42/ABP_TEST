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
    public async Task<ActionResult<List<HallResponseDto>>> GetAll()
    {
        return Ok(await _hallService.GetAllAsync());
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HallResponseDto>> GetById(int id)
    {
        return Ok(await _hallService.GetByIdAsync(id));
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(List<HallResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<HallResponseDto>>> Search([FromQuery] HallSearchRequestDto request)
    {
        return Ok(await _hallService.SearchAvailableAsync(request));
    }

    [HttpPost]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HallResponseDto>> Create(CreateHallDto createHallDto)
    {
        var hall = await _hallService.CreateAsync(createHallDto);
        return CreatedAtAction(nameof(GetById), new { id = hall.Id }, hall);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HallResponseDto>> Update(int id, UpdateHallDto updateHallDto)
    {
        return Ok(await _hallService.UpdateAsync(id, updateHallDto));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        await  _hallService.DeleteAsync(id);
        return NoContent();
    }
    
    
}