using System.Net.Http.Json;
using TenderingSystem.Shared.Models.Dashboard;

namespace TenderingSystem.BlazorClient.Services;

public class DashboardService
{
    private readonly HttpClient _httpClient;

    public DashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardStatsDto>("api/dashboard");
        }
        catch
        {
            return null;
        }
    }
}
