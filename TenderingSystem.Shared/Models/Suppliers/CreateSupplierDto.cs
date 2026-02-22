namespace TenderingSystem.Shared.Models.Suppliers;

public class CreateSupplierDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public List<Guid>? CategoryIds { get; set; }
}
