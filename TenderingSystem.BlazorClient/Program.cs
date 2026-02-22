using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TenderingSystem.BlazorClient;

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using TenderingSystem.BlazorClient.Providers;
using TenderingSystem.BlazorClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Connect to the Web API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5100/") });

// Register MudBlazor
builder.Services.AddMudServices();

// Register Auth state provider & Local Storage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
