using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Infrastructure.Data;

namespace TenderingSystem.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
