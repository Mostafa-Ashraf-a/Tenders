using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Infrastructure.Data;

namespace TenderingSystem.Infrastructure.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Supplier?> GetByEmailAsync(string email)
    {
        return await _dbContext.Suppliers
            .Include(s => s.SupplierCategories)
            .ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public new async Task<IReadOnlyList<Supplier>> GetAllAsync()
    {
        return await _dbContext.Suppliers
            .Include(s => s.SupplierCategories)
            .ThenInclude(sc => sc.Category)
            .ToListAsync();
    }

    public new async Task<Supplier?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Suppliers
            .Include(s => s.SupplierCategories)
            .ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
