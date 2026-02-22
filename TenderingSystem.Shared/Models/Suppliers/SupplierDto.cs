namespace TenderingSystem.Shared.Models.Suppliers;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AiExtractedNotes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // For displaying the names in UI
    public List<string> CategoryNames { get; set; } = new();
    
    // For selecting in edit forms
    public List<Guid> CategoryIds { get; set; } = new();
}
