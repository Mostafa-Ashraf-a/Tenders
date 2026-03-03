using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Shared.Models.Categories;

namespace TenderingSystem.Infrastructure.Services;

public class GeminiAnalysisService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAnalysisService> _logger;

    public GeminiAnalysisService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAnalysisService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AiExtractedTenderDto?> AnalyzeTenderTextAsync(string rawText, List<CategoryDto> availableCategories)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Gemini API Key is not configured in appsettings.");
            return null;
        }

        var categoriesContext = string.Join(", ", availableCategories.Select(c => $"[Id: {c.Id}, Name: {c.Name}]"));

        var prompt = $@"
You are an expert procurement and tender data extraction assistant.
I will provide you with raw text extracted from a tender/bidding website.
Your job is to extract the following information and return ONLY a valid JSON object matching this schema:
{{
  ""Title"": ""The title or name of the tender"",
  ""Description"": ""A brief description or summary of what is required"",
  ""ClosingDate"": ""The deadline or closing date in ISO 8601 format (YYYY-MM-DDTHH:mm:ssZ) if found, otherwise null"",
  ""CategoryId"": ""Select the most appropriate Category Id from the Available Categories list. If none match, return null.""
}}

Available Categories: {categoriesContext}

RAW TEXT TO ANALYZE:
---
{rawText}
---
        ";

        var requestBody = new
        {
            contents = new[]
            {
                new 
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(requestUrl, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API call failed: {Error}", error);
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            
            // Extract the text part from the Gemini response structure
            var responseText = jsonDocument
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(responseText)) return null;

            // Sometimes Gemini wraps JSON in markdown blocks (```json ... ```)
            var cleanJson = responseText.Replace("```json", "").Replace("```", "").Trim();

            var extractedData = JsonSerializer.Deserialize<AiExtractedTenderDto>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return extractedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calling Gemini API.");
            return null;
        }
    }

    public async Task<List<SupplierLeadDto>> ExtractSuppliersFromSearchAsync(string searchContent)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Gemini API Key is not configured in appsettings.");
            return new List<SupplierLeadDto>();
        }

        var prompt = $@"
You are a lead generation and data extraction assistant.
I will provide you with raw text extracted from a search engine result page (like Google or DuckDuckGo) where I searched for companies.
Your job is to identify valid companies and their email addresses. Ignore any results that do not have a company name and a valid email address.
Return ONLY a valid JSON array of objects matching this exact schema:
[
  {{
    ""CompanyName"": ""The extracted name of the company or supplier"",
    ""Email"": ""extracted.email@example.com""
  }}
]
If you find no valid companies with emails, return an empty array [].

RAW SEARCH TEXT:
---
{searchContent}
---
        ";

        var requestBody = new
        {
            contents = new[]
            {
                new 
                {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(requestUrl, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API call failed for Lead Generation: {Error}", error);
                return new List<SupplierLeadDto>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            
            var responseText = jsonDocument
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(responseText)) return new List<SupplierLeadDto>();

            var cleanJson = responseText.Replace("```json", "").Replace("```", "").Trim();

            var extractedData = JsonSerializer.Deserialize<List<SupplierLeadDto>>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return extractedData ?? new List<SupplierLeadDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calling Gemini API for Lead Generation.");
            return new List<SupplierLeadDto>();
        }
    }
}
