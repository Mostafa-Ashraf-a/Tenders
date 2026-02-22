using System.Net.Http.Json;
using TenderingSystem.Shared.Models.Suppliers;

namespace TenderingSystem.BlazorClient.Services;

public class SupplierService
{
    private readonly HttpClient _httpClient;

    public SupplierService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SupplierDto>> GetAllSuppliersAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<SupplierDto>>("api/suppliers");
        return result ?? new List<SupplierDto>();
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<SupplierDto>($"api/suppliers/{id}");
    }

    public async Task<SupplierDto?> CreateSupplierAsync(CreateSupplierDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/suppliers", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SupplierDto>();
    }

    public async Task<SupplierDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/suppliers/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SupplierDto>();
    }

    public async Task<bool> DeleteSupplierAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/suppliers/{id}");
        return response.IsSuccessStatusCode;
    }
}
