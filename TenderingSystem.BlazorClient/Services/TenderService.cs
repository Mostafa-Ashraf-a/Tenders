using System.Net.Http.Json;
using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.BlazorClient.Services;

public class TenderService
{
    private readonly HttpClient _httpClient;

    public TenderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TenderDto>> GetAllTendersAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<TenderDto>>("api/tenders");
        return result ?? new List<TenderDto>();
    }

    public async Task<TenderDto?> GetTenderByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<TenderDto>($"api/tenders/{id}");
    }

    public async Task<TenderDto?> CreateTenderAsync(CreateTenderDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tenders", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TenderDto>();
    }

    public async Task<TenderDto?> UpdateTenderAsync(Guid id, UpdateTenderDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tenders/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<TenderDto>();
    }

    public async Task<bool> DeleteTenderAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/tenders/{id}");
        return response.IsSuccessStatusCode;
    }
}
