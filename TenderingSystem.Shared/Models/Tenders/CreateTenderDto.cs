namespace TenderingSystem.Shared.Models.Tenders;

public class CreateTenderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public bool HasAiTargeting { get; set; }
}
