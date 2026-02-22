using TenderingSystem.Domain.Enums;

namespace TenderingSystem.Domain.Entities;

public class Tender
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., "IT Equipment", "Furniture"
    public DateTime PublishDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public TenderStatus Status { get; set; } = TenderStatus.Draft;
    public bool HasAiTargeting { get; set; } = false; // Indicates if AI search is enabled for this tender

    // Navigation properties
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public ICollection<AiSearchLog> AiSearchLogs { get; set; } = new List<AiSearchLog>();
}
