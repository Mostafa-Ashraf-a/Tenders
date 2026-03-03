using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Infrastructure.Data;
using TenderingSystem.Shared.Models.Dashboard;
using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var stats = new DashboardStatsDto();

        // 1. Total Tenders
        stats.TotalTenders = await _dbContext.Tenders.CountAsync(t => t.Status != TenderStatus.Draft);

        // 2. Total Suppliers
        stats.TotalSuppliers = await _dbContext.Suppliers.CountAsync();

        // 3. Total AI Generated Suppliers
        stats.TotalAiGeneratedSuppliers = await _dbContext.Suppliers.CountAsync(s => s.Status == SupplierStatus.AiSuggested);

        // 4. Last 3 published Tenders
        var recentTenders = await _dbContext.Tenders
            .Include(t => t.Category)
            .Where(t => t.Status == TenderStatus.Published)
            .OrderByDescending(t => t.PublishDate)
            .Take(3)
            .ToListAsync();

        stats.RecentTenders = recentTenders.Select(t => new TenderDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name ?? string.Empty,
            PublishDate = t.PublishDate,
            ClosingDate = t.ClosingDate,
            Status = t.Status.ToString(),
            HasAiTargeting = t.HasAiTargeting
        }).ToList();

        // 5. Top Active Suppliers (by Bids)
        var topSuppliers = await _dbContext.Suppliers
            .Include(s => s.Bids)
            .Where(s => s.Bids.Any())
            .OrderByDescending(s => s.Bids.Count)
            .Take(5)
            .Select(s => new ActiveSupplierDto
            {
                CompanyName = s.CompanyName,
                Email = s.Email,
                BidsCount = s.Bids.Count
            })
            .ToListAsync();

        stats.TopActiveSuppliers = topSuppliers;

        return stats;
    }
}
