using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Infrastructure.Data;
using TenderingSystem.Infrastructure.Repositories;

namespace TenderingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITenderRepository, TenderRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IAiSearchLogRepository, AiSearchLogRepository>();

        return services;
    }
}
