namespace TenderingSystem.Shared.Models.Tenders;

public class CreateTenderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; } = DateTime.Today;
    public DateTime ClosingDate { get; set; } = DateTime.Today.AddDays(30);
    public bool HasAiTargeting { get; set; } = false;
}
