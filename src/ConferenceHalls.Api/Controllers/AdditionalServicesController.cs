using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Controllers;

[ApiController]
[Route("api/halls/{hallId:int}/services")]
public class AdditionalServicesController : ControllerBase
{
    private readonly IAdditionalServiceService _additionalServiceService;

    public AdditionalServicesController(IAdditionalServiceService additionalServiceService)
    {
        _additionalServiceService = additionalServiceService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AdditionalServiceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AdditionalServiceResponseDto>>> GetAll(int hallId)
    {
        return Ok(await _additionalServiceService.GetAllAsync(hallId));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AdditionalServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdditionalServiceResponseDto>> GetById(int hallId, int id)
    {
        return Ok(await _additionalServiceService.GetByIdAsync(hallId, id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdditionalServiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdditionalServiceResponseDto>> Create(int hallId, AdditionalServiceRequestDto dto)
    {
        var service = await _additionalServiceService.CreateAsync(hallId, dto);
        return CreatedAtAction(nameof(GetById), new { hallId, id = service.Id }, service);
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AdditionalServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdditionalServiceResponseDto>> Update(int hallId, int id, AdditionalServiceRequestDto dto)
    {
        return Ok(await _additionalServiceService.UpdateAsync(hallId, id, dto));
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int hallId, int id)
    {
        await _additionalServiceService.DeleteAsync(hallId, id);
        return NoContent();
    }
}