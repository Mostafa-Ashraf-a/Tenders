using TenderingSystem.Shared.Models.Dashboard;

namespace TenderingSystem.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
