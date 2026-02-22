namespace TenderingSystem.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation
    public ICollection<Tender> Tenders { get; set; } = new List<Tender>();
    public ICollection<SupplierCategory> SupplierCategories { get; set; } = new List<SupplierCategory>();
}
