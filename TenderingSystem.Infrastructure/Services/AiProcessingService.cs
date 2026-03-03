using Microsoft.Extensions.Logging;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;

namespace TenderingSystem.Infrastructure.Services;

public class AiProcessingService : IAiProcessingService
{
    private readonly IWebScraperService _scraperService;
    private readonly IGeminiService _geminiService;
    private readonly ICategoryService _categoryService;
    private readonly ITenderRepository _tenderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<AiProcessingService> _logger;

    public AiProcessingService(
        IWebScraperService scraperService, 
        IGeminiService geminiService, 
        ICategoryService categoryService,
        ITenderRepository tenderRepository,
        ISupplierRepository supplierRepository,
        IEmailService emailService,
        ILogger<AiProcessingService> logger)
    {
        _scraperService = scraperService;
        _geminiService = geminiService;
        _categoryService = categoryService;
        _tenderRepository = tenderRepository;
        _supplierRepository = supplierRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task TestScrapeAsync(string url)
    {
        _logger.LogInformation("Starting AI Scrape for URL: {Url}", url);
        try
        {
            // 1. Scrape raw text
            var rawText = await _scraperService.ScrapeTenderPageTextAsync(url);
            _logger.LogInformation("Successfully scraped {Length} characters from {Url}", rawText.Length, url);
            
            // 2. Get available categories for Gemini to map to
            var categories = await _categoryService.GetAllCategoriesAsync();

            // 3. Send to Gemini for Analysis
            _logger.LogInformation("Sending data to Gemini for analysis...");
            var extractedDto = await _geminiService.AnalyzeTenderTextAsync(rawText, categories.ToList());

            if (extractedDto == null)
            {
                _logger.LogWarning("Gemini failed to extract data or returned null.");
                return;
            }

            _logger.LogInformation("Gemini Analysis Success! Title: {Title}, Category: {CategoryId}", extractedDto.Title, extractedDto.CategoryId);

            // 4. Save the tender to the database (mark as AiSuggested)
            var tender = new Tender
            {
                Title = string.IsNullOrWhiteSpace(extractedDto.Title) ? "مجهول (تم السحب)" : extractedDto.Title,
                Description = string.IsNullOrWhiteSpace(extractedDto.Description) ? "لا يوجد وصف" : extractedDto.Description,
                PublishDate = DateTime.UtcNow,
                ClosingDate = extractedDto.ClosingDate ?? DateTime.UtcNow.AddDays(7),
                Status = TenderStatus.AiSuggested, 
                CategoryId = extractedDto.CategoryId
            };

            await _tenderRepository.AddAsync(tender);
            _logger.LogInformation("Tender successfully saved to database with ID: {Id}", tender.Id);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing AI Scraping Workflow for URL: {Url}", url);
        }
    }

    public async Task GenerateLeadsForTenderAsync(Guid tenderId)
    {
        _logger.LogInformation("Starting AI Lead Generation for Tender ID: {TenderId}", tenderId);

        try
        {
            // 1. Get the tender
            var tender = await _tenderRepository.GetByIdAsync(tenderId);
            if (tender == null || tender.CategoryId == null)
            {
                _logger.LogWarning("Tender not found or has no category. Aborting Lead Gen.");
                return;
            }

            // 2. Get the category to know what to search for
            var categories = await _categoryService.GetAllCategoriesAsync();
            var category = categories.FirstOrDefault(c => c.Id == tender.CategoryId);
            if (category == null) return;

            // 3. Search the web using Playwright
            _logger.LogInformation("Searching duckduckgo for category: {CategoryName}", category.Name);
            var searchResultsText = await _scraperService.SearchSuppliersAsync(category.Name);

            // 4. Send to Gemini to extract Supplier Names and Emails
            _logger.LogInformation("Extracting leads using Gemini...");
            var leads = await _geminiService.ExtractSuppliersFromSearchAsync(searchResultsText);

            if (leads == null || !leads.Any())
            {
                _logger.LogInformation("No valid leads found in search results.");
                return;
            }

            _logger.LogInformation("Found {Count} potential leads. Processing...", leads.Count);

            foreach (var lead in leads)
            {
                // Validate email basic structure just in case
                if (string.IsNullOrWhiteSpace(lead.Email) || !lead.Email.Contains("@")) continue;

                var existingSupplier = await _supplierRepository.GetByEmailAsync(lead.Email);
                if (existingSupplier == null)
                {
                    // Create new AiSuggested supplier
                    var newSupplier = new Supplier
                    {
                        CompanyName = lead.CompanyName,
                        Email = string.IsNullOrWhiteSpace(lead.Email) ? string.Empty : lead.Email,
                        AiExtractedNotes = "AI Suggested Lead",
                        Status = SupplierStatus.AiSuggested
                    };

                    await _supplierRepository.AddAsync(newSupplier);
                    _logger.LogInformation("Saved new AI Suggested Supplier: {CompanyName} ({Email})", newSupplier.CompanyName, newSupplier.Email);

                    // Send marketing email
                    var subject = $"دعوة مستهدفة: فرصة لتوريد {category.Name}";
                    var body = $@"
                        <h3>مرحباً فريق {lead.CompanyName} الموقر،</h3>
                        <p>وجدنا أن نشاطكم التجاري يتوافق مع مناقصة مطروحة حالياً بعنوان <strong>{tender.Title}</strong>.</p>
                        <p>يسعدنا انضمامكم لمنصتنا والتسجيل لتقديم عرضكم والمنافسة بقوة. نحن بانتظاركم!</p>
                        <p>مع تحيات نظام المناقصات.</p>
                    ";

                    await _emailService.SendEmailAsync(lead.Email, subject, body);
                    _logger.LogInformation("Sent marketing email to: {Email}", lead.Email);
                }
            }

            _logger.LogInformation("AI Lead Generation workflow completed successfully for Tender {TenderId}", tenderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing AI Lead Generation for Tender {TenderId}", tenderId);
        }
    }
}
