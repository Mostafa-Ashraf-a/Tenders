using TenderingSystem.Domain.Enums;

namespace TenderingSystem.Domain.Entities;

public class Supplier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public SupplierStatus Status { get; set; } = SupplierStatus.AiSuggested;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Notes extracted by AI from the website
    public string AiExtractedNotes { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}
