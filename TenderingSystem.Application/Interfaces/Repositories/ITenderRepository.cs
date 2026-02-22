using TenderingSystem.Domain.Entities;

namespace TenderingSystem.Application.Interfaces.Repositories;

public interface ITenderRepository : IGenericRepository<Tender>
{
    Task<IReadOnlyList<Tender>> GetActiveTendersAsync();
}
