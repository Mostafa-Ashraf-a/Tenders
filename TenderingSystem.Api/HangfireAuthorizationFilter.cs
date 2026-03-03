using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace TenderingSystem.Api;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // For security, checking if the user is authenticated and is an Admin.
        // During testing/development, you might want to return true to skip Auth.
        // return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");
        
        // Allowed for testing right now
        return true;
    }
}
