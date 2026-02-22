using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Infrastructure.Data;

namespace TenderingSystem.Infrastructure.Repositories;

public class BidRepository : GenericRepository<Bid>, IBidRepository
{
    public BidRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Bid>> GetBidsByTenderIdAsync(Guid tenderId)
    {
        return await _dbContext.Bids
            .Where(b => b.TenderId == tenderId)
            .Include(b => b.Supplier)
            .ToListAsync();
    }
}
