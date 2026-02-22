using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Shared.Models.Bids;

namespace TenderingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BidsController : ControllerBase
{
    private readonly IBidService _bidService;
    private readonly ISupplierRepository _supplierRepository;

    public BidsController(IBidService bidService, ISupplierRepository supplierRepository)
    {
        _bidService = bidService;
        _supplierRepository = supplierRepository;
    }

    [HttpGet("tender/{tenderId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetBidsForTender(Guid tenderId)
    {
        try
        {
            var bids = await _bidService.GetBidsByTenderIdAsync(tenderId);
            return Ok(bids);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-bids")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> GetMyBids()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suppliers = await _supplierRepository.GetAsync(s => s.UserId == userId);
            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
                return NotFound("Supplier profile not found.");

            var bids = await _bidService.GetBidsBySupplierIdAsync(supplier.Id);
            return Ok(bids);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> SubmitBid([FromBody] CreateBidDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suppliers = await _supplierRepository.GetAsync(s => s.UserId == userId);
            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
                return NotFound("Supplier profile not found.");

            var bid = await _bidService.SubmitBidAsync(supplier.Id, dto);
            return CreatedAtAction(nameof(GetMyBids), new { id = bid.Id }, bid);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/award")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AwardBid(Guid id)
    {
        try
        {
            await _bidService.AwardBidAsync(id);
            return Ok(new { message = "Bid awarded successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
