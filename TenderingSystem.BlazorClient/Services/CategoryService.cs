using System.Net.Http.Json;
using TenderingSystem.Shared.Models.Categories;

namespace TenderingSystem.BlazorClient.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return result ?? new List<CategoryDto>();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
    }

    public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        return response.IsSuccessStatusCode;
    }
}
