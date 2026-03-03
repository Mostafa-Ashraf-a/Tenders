using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using TenderingSystem.Infrastructure.Data;
using TenderingSystem.Infrastructure.Identity;
using TenderingSystem.Infrastructure.Repositories;
using TenderingSystem.Infrastructure.Services;
using Hangfire;

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

        services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<TenderingSystem.Shared.Models.Auth.JwtSettings>(
            configuration.GetSection("JwtSettings"));

        services.AddScoped<IAuthService, TenderingSystem.Infrastructure.Identity.AuthService>();
        services.AddScoped<ITenderService, TenderService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IBidService, BidService>();
        services.AddScoped<IBackgroundJobService, HangfireJobService>();
        services.AddScoped<IWebScraperService, PlaywrightScraperService>();
        services.AddHttpClient<IGeminiService, GeminiAnalysisService>();
        services.AddScoped<IAiProcessingService, AiProcessingService>();

        // Add Hangfire services
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        return services;
    }
}
