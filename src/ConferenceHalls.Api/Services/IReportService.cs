using ConferenceHalls.Api.DTO;

namespace ConferenceHalls.Api.Services;

public interface IReportService
{
    Task<RevenueReportResponseDto> GetRevenueAsync(ReportPeriodRequestDto request);
    Task<UtilizationReportResponseDto> GetUtilizationAsync(ReportPeriodRequestDto request);
}
