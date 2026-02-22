namespace TenderingSystem.Shared.Models.Tenders;

public class UpdateTenderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool HasAiTargeting { get; set; }
}
