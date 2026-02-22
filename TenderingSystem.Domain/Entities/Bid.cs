using TenderingSystem.Domain.Enums;

namespace TenderingSystem.Domain.Entities;

public class Bid
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TenderId { get; set; }
    public Tender Tender { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public decimal SubmittedPrice { get; set; }
    public string TechnicalProposalUrl { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public BidStatus Status { get; set; } = BidStatus.Submitted;
}
