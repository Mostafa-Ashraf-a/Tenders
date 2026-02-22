using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Infrastructure.Data;

namespace TenderingSystem.Infrastructure.Repositories;

public class TenderRepository : GenericRepository<Tender>, ITenderRepository
{
    public TenderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Tender>> GetActiveTendersAsync()
    {
        return await _dbContext.Tenders
            .Where(t => t.Status == TenderStatus.Published)
            .ToListAsync();
    }
}
