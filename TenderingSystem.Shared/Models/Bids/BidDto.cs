namespace TenderingSystem.Shared.Models.Bids;

public class BidDto
{
    public Guid Id { get; set; }
    public Guid TenderId { get; set; }
    public string TenderTitle { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal SubmittedPrice { get; set; }
    public string TechnicalProposalUrl { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
