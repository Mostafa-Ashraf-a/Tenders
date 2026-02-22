using TenderingSystem.Domain.Entities;

namespace TenderingSystem.Application.Interfaces.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<Supplier?> GetByEmailAsync(string email);
}
