using System.Net.Http.Json;
using TenderingSystem.Shared.Models.Bids;

namespace TenderingSystem.BlazorClient.Services;

public class BidService
{
    private readonly HttpClient _httpClient;

    public BidService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BidDto>> GetMyBidsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<BidDto>>("api/bids/my-bids") ?? new List<BidDto>();
    }

    public async Task<List<BidDto>> GetBidsForTenderAsync(Guid tenderId)
    {
        return await _httpClient.GetFromJsonAsync<List<BidDto>>($"api/bids/tender/{tenderId}") ?? new List<BidDto>();
    }

    public async Task<BidDto> SubmitBidAsync(CreateBidDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/bids", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BidDto>() ?? throw new Exception("Failed to submit bid.");
    }

    public async Task<bool> HasSupplierBidAsync(Guid tenderId)
    {
        return await _httpClient.GetFromJsonAsync<bool>($"api/bids/tender/{tenderId}/has-bid");
    }

    public async Task AwardBidAsync(Guid bidId)
    {
        var response = await _httpClient.PostAsync($"api/bids/{bidId}/award", null);
        response.EnsureSuccessStatusCode();
    }
}
