using TenderingSystem.Domain.Enums;

namespace TenderingSystem.Domain.Entities;

public class Tender
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public TenderStatus Status { get; set; } = TenderStatus.Draft;
    public bool HasAiTargeting { get; set; } = false;

    // Navigation properties
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public ICollection<AiSearchLog> AiSearchLogs { get; set; } = new List<AiSearchLog>();
}
