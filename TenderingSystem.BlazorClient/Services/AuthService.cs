using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TenderingSystem.Shared.Models.Auth;
using TenderingSystem.BlazorClient.Providers;

namespace TenderingSystem.BlazorClient.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login Failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result == null || string.IsNullOrWhiteSpace(result.Token))
        {
            throw new Exception("Invalid response from server");
        }

        await _localStorage.SetItemAsync("authToken", result.Token);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.Token);

        return result;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Registration Failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result == null || string.IsNullOrWhiteSpace(result.Token))
        {
            throw new Exception("Invalid response from server");
        }

        await _localStorage.SetItemAsync("authToken", result.Token);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.Token);

        return result;
    }
}
