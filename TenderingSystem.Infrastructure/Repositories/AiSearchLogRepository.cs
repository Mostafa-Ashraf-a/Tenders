using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Infrastructure.Data;

namespace TenderingSystem.Infrastructure.Repositories;

public class AiSearchLogRepository : GenericRepository<AiSearchLog>, IAiSearchLogRepository
{
    public AiSearchLogRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<AiSearchLog>> GetLogsByTenderIdAsync(Guid tenderId)
    {
        return await _dbContext.AiSearchLogs
            .Where(l => l.TenderId == tenderId)
            .OrderByDescending(l => l.StartedAt)
            .ToListAsync();
    }
}
