using System.ComponentModel.DataAnnotations;

namespace TenderingSystem.Shared.Models.Bids;

public class CreateBidDto
{
    [Required]
    public Guid TenderId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal SubmittedPrice { get; set; }

    [Required]
    [Url(ErrorMessage = "Please provide a valid URL for the technical proposal.")]
    public string TechnicalProposalUrl { get; set; } = string.Empty;
}
