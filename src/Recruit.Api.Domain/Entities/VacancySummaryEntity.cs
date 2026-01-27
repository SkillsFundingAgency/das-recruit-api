using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class VacancySummaryEntity
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public long? VacancyReference { get; set; }
    public string? LegalEntityName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public VacancyStatus Status { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public ApplicationMethod? ApplicationMethod { get; set; }
    public ApprenticeshipTypes? ApprenticeshipType { get; set; }
    public string? TransferInfo { get; set; }
    public bool HasSubmittedAdditionalQuestions { get; set; }
    public OwnerType? OwnerType { get; set; }
    public int? Ukprn { get; set; }
    public SourceOrigin? SourceOrigin { get; set; }
}