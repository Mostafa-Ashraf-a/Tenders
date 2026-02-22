using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenderingSystem.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using TenderingSystem.Infrastructure.Data;
using TenderingSystem.Infrastructure.Identity;
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

        services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<TenderingSystem.Application.Models.Auth.JwtSettings>(
            configuration.GetSection("JwtSettings"));

        services.AddScoped<TenderingSystem.Application.Interfaces.Services.IAuthService, TenderingSystem.Infrastructure.Identity.AuthService>();

        return services;
    }
}
