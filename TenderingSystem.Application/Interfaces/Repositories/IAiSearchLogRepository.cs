using TenderingSystem.Domain.Entities;

namespace TenderingSystem.Application.Interfaces.Repositories;

public interface IAiSearchLogRepository : IGenericRepository<AiSearchLog>
{
    Task<IReadOnlyList<AiSearchLog>> GetLogsByTenderIdAsync(Guid tenderId);
}
