namespace TenderingSystem.Application.Interfaces.Services;

public interface IAiProcessingService
{
    Task TestScrapeAsync(string url);

    /// <summary>
    /// AI Workflow for Lead Generation: Searches web for suppliers based on the tender category, extracts leads using Gemini, saves them, and sends a marketing email.
    /// </summary>
    Task GenerateLeadsForTenderAsync(Guid tenderId);
}
