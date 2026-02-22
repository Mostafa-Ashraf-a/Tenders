namespace TenderingSystem.Domain.Entities;

public class SupplierCategory
{
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
