using TenderingSystem.Shared.Models.Categories;

namespace TenderingSystem.Application.Interfaces.Services;

public class AiExtractedTenderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ClosingDate { get; set; }
    public Guid? CategoryId { get; set; }
}

public class SupplierLeadDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public interface IGeminiService
{
    /// <summary>
    /// Analyzes raw text from a tender page and extracts structured data based on available categories.
    /// </summary>
    Task<AiExtractedTenderDto?> AnalyzeTenderTextAsync(string rawText, List<CategoryDto> availableCategories);

    /// <summary>
    /// Extracts supplier names and emails from search engine results text.
    /// </summary>
    Task<List<SupplierLeadDto>> ExtractSuppliersFromSearchAsync(string searchContent);
}
