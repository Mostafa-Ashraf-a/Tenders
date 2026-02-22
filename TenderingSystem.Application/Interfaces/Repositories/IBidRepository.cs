using TenderingSystem.Domain.Entities;

namespace TenderingSystem.Application.Interfaces.Repositories;

public interface IBidRepository : IGenericRepository<Bid>
{
    Task<IReadOnlyList<Bid>> GetBidsByTenderIdAsync(Guid tenderId);
}
