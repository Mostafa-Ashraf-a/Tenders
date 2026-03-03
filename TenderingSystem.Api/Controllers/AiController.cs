using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenderingSystem.Application.Interfaces.Services;

namespace TenderingSystem.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // Optional: restrict this testing endpoint to Admins only
public class AiController : ControllerBase
{
    private readonly IBackgroundJobService _backgroundJobService;

    public AiController(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public class TestScrapeRequest
    {
        public string Url { get; set; } = string.Empty;
    }

    [HttpPost("extract-tender")]
    public IActionResult ExtractTender([FromBody] TestScrapeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Url))
            return BadRequest(new { message = "URL is required." });

        // Enqueue a background job that uses DI to resolve IAiProcessingService
        var jobId = _backgroundJobService.Enqueue<IAiProcessingService>(x => x.TestScrapeAsync(request.Url));

        return Ok(new { message = "AI Scraping and Extraction workflow enqueued successfully.", jobId });
    }
}
