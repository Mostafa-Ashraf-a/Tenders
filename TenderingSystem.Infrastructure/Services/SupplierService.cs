using Microsoft.EntityFrameworkCore;
using TenderingSystem.Application.Interfaces.Repositories;
using TenderingSystem.Application.Interfaces.Services;
using TenderingSystem.Domain.Entities;
using TenderingSystem.Domain.Enums;
using TenderingSystem.Shared.Models.Suppliers;

namespace TenderingSystem.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToDto).ToList();
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier == null ? null : MapToDto(supplier);
    }

    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            CompanyName = dto.CompanyName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            WebsiteUrl = dto.WebsiteUrl,
            Status = SupplierStatus.Registered,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            foreach (var categoryId in dto.CategoryIds)
            {
                supplier.SupplierCategories.Add(new SupplierCategory
                {
                    CategoryId = categoryId
                });
            }
        }

        await _supplierRepository.AddAsync(supplier);
        return MapToDto(supplier);
    }

    public async Task<SupplierDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) return null;

        supplier.CompanyName = dto.CompanyName;
        supplier.Email = dto.Email;
        supplier.PhoneNumber = dto.PhoneNumber;
        supplier.WebsiteUrl = dto.WebsiteUrl;
        supplier.AiExtractedNotes = dto.AiExtractedNotes;

        if (Enum.TryParse<SupplierStatus>(dto.Status, true, out var status))
            supplier.Status = status;

        // Update Categories
        supplier.SupplierCategories.Clear();
        if (dto.CategoryIds != null && dto.CategoryIds.Any())
        {
            foreach (var categoryId in dto.CategoryIds)
            {
                supplier.SupplierCategories.Add(new SupplierCategory
                {
                    SupplierId = supplier.Id,
                    CategoryId = categoryId
                });
            }
        }

        await _supplierRepository.UpdateAsync(supplier);
        return MapToDto(supplier);
    }

    public async Task<bool> DeleteSupplierAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) return false;

        await _supplierRepository.DeleteAsync(supplier);
        return true;
    }

    private static SupplierDto MapToDto(Supplier supplier) => new()
    {
        Id = supplier.Id,
        CompanyName = supplier.CompanyName,
        Email = supplier.Email,
        PhoneNumber = supplier.PhoneNumber,
        WebsiteUrl = supplier.WebsiteUrl,
        Status = supplier.Status.ToString(),
        AiExtractedNotes = supplier.AiExtractedNotes,
        CreatedAt = supplier.CreatedAt,
        CategoryIds = supplier.SupplierCategories.Select(sc => sc.CategoryId).ToList(),
        CategoryNames = supplier.SupplierCategories.Select(sc => sc.Category?.Name ?? string.Empty).Where(n => !string.IsNullOrEmpty(n)).ToList()
    };
}
