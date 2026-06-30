namespace SFA.DAS.Recruit.Api.Domain.Models;

public class ManualQaEditFieldIndicator
{
    public string FieldIdentifier { get; set; }
    public string? BeforeEdit { get; set; } = null;
    public string? AfterEdit { get; set; } = null;
}