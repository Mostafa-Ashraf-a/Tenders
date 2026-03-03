using Microsoft.Playwright;
using TenderingSystem.Application.Interfaces.Services;

namespace TenderingSystem.Infrastructure.Services;

public class PlaywrightScraperService : IWebScraperService
{
    public async Task<string> ScrapeTenderPageTextAsync(string url)
    {
        using var playwright = await Playwright.CreateAsync();
        // Typically we use Chromium for scraping with headless = true
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            // Navigate to the target page and wait for it to be fully loaded
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = 60000 // 60 seconds timeout
            });

            // Extract the inner text of the entire body. 
            // In a real scenario, you might want to target a specific selector to reduce noise.
            var content = await page.InnerTextAsync("body");
            
            return content;
        }
        catch (Exception ex)
        {
            // Log the error (can inject ILogger here if needed)
            throw new Exception($"Failed to scrape URL {url}. Error: {ex.Message}");
        }
    }

    public async Task<string> SearchSuppliersAsync(string categoryName)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            // Using DuckDuckGo HTML version to avoid heavy JS and captchas
            var query = Uri.EscapeDataString($"شركات {categoryName} بريد إلكتروني email contact");
            var url = $"https://html.duckduckgo.com/html/?q={query}";

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = 60000 
            });

            // Extract the result snippets
            // The search results on DuckDuckGo HTML are in '.result__snippet' and '.result__title'
            var resultsText = await page.InnerTextAsync("body");
            
            return resultsText;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to search suppliers for category {categoryName}. Error: {ex.Message}");
        }
    }
}
