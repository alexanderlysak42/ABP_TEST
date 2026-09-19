using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BookingResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
    {
        return Ok(await _bookingService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponseDto>> GetById(int id)
    {
        return Ok(await _bookingService.GetByIdAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponseDto>> Create(BookingRequestDto request)
    {
        var booking = await _bookingService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }
}