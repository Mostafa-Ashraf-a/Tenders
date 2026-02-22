using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Shared.Models.Tenders;

namespace TenderingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TendersController : ControllerBase
{
    private readonly ITenderService _tenderService;

    public TendersController(ITenderService tenderService)
    {
        _tenderService = tenderService;
    }

    // GET: api/tenders
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenderDto>>> GetAll()
    {
        var tenders = await _tenderService.GetAllTendersAsync();
        return Ok(tenders);
    }

    // GET: api/tenders/active
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<TenderDto>>> GetActive()
    {
        var tenders = await _tenderService.GetActiveTendersAsync();
        return Ok(tenders);
    }

    // GET: api/tenders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenderDto>> GetById(Guid id)
    {
        var tender = await _tenderService.GetTenderByIdAsync(id);
        if (tender == null) return NotFound();
        return Ok(tender);
    }

    // POST: api/tenders
    [HttpPost]
    public async Task<ActionResult<TenderDto>> Create([FromBody] CreateTenderDto dto)
    {
        var tender = await _tenderService.CreateTenderAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tender.Id }, tender);
    }

    // PUT: api/tenders/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenderDto>> Update(Guid id, [FromBody] UpdateTenderDto dto)
    {
        var tender = await _tenderService.UpdateTenderAsync(id, dto);
        if (tender == null) return NotFound();
        return Ok(tender);
    }

    // DELETE: api/tenders/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _tenderService.DeleteTenderAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // POST: api/tenders/{id}/publish
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var published = await _tenderService.PublishTenderAsync(id);
        if (!published) return BadRequest("التأكد من أن المناقصة مسودة قبل نشرها");
        return Ok();
    }
}
