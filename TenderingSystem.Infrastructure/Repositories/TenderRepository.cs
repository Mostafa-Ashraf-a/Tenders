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
            .Include(t => t.Category)
            .Where(t => t.Status == TenderStatus.Published)
            .ToListAsync();
    }

    public new async Task<IReadOnlyList<Tender>> GetAllAsync()
    {
        return await _dbContext.Tenders
            .Include(t => t.Category)
            .ToListAsync();
    }

    public new async Task<Tender?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Tenders
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
