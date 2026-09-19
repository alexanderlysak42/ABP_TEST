using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueReportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RevenueReportResponseDto>> Revenue([FromQuery] ReportPeriodRequestDto request)
    {
        return Ok(await _reportService.GetRevenueAsync(request));
    }

    [HttpGet("utilization")]
    [ProducesResponseType(typeof(UtilizationReportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UtilizationReportResponseDto>> Utilization([FromQuery] ReportPeriodRequestDto request)
    {
        return Ok(await _reportService.GetUtilizationAsync(request));
    }
}
