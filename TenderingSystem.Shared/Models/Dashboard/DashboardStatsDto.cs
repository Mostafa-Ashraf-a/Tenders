using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Shared.Models.Dashboard;

public class ActiveSupplierDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int BidsCount { get; set; }
}

public class DashboardStatsDto
{
    public int TotalTenders { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalAiGeneratedSuppliers { get; set; }
    public List<TenderDto> RecentTenders { get; set; } = new();
    public List<ActiveSupplierDto> TopActiveSuppliers { get; set; } = new();
}
