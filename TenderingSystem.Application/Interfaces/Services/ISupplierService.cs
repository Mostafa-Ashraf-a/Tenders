using TenderingSystem.Shared.Models.Suppliers;

namespace TenderingSystem.Application.Interfaces.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierDto>> GetAllSuppliersAsync();
    Task<SupplierDto?> GetSupplierByIdAsync(Guid id);
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto);
    Task<SupplierDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto dto);
    Task<bool> DeleteSupplierAsync(Guid id);
}
