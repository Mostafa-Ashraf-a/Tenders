namespace TenderingSystem.Application.Interfaces.Services;

public interface IWebScraperService
{
    /// <summary>
    /// Opens the specified URL in a headless browser and extracts the entire inner text of the page.
    /// </summary>
    /// <param name="url">The URL of the tender page to scrape.</param>
    /// <returns>The raw text content of the page.</returns>
    Task<string> ScrapeTenderPageTextAsync(string url);

    /// <summary>
    /// Searches the web for suppliers in a specific category and returns the raw text of the search results.
    /// </summary>
    /// <param name="categoryName">The name of the category (e.g., 'Hospital Equipment Providers in Egypt').</param>
    /// <returns>The raw text of the search results page.</returns>
    Task<string> SearchSuppliersAsync(string categoryName);
}
