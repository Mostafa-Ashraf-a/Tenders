using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Shared.Models.Suppliers;

namespace TenderingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: api/suppliers
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SupplierDto>>> GetAll()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return Ok(suppliers);
    }

    // GET: api/suppliers/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierDto>> GetById(Guid id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        if (supplier == null) return NotFound();
        return Ok(supplier);
    }

    // POST: api/suppliers
    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierDto dto)
    {
        var supplier = await _supplierService.CreateSupplierAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    // PUT: api/suppliers/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SupplierDto>> Update(Guid id, [FromBody] UpdateSupplierDto dto)
    {
        var supplier = await _supplierService.UpdateSupplierAsync(id, dto);
        if (supplier == null) return NotFound();
        return Ok(supplier);
    }

    // DELETE: api/suppliers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _supplierService.DeleteSupplierAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
