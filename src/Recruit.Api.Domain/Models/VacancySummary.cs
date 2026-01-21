using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Models;

public record VacancySummary
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public long? VacancyReference { get; set; }
    public string? LegalEntityName { get; set; }
    public string? AccountLegalEntityId { get; set; }
    public string? EmployerAccountId { get; set; }
    public string? EmployerName { get; set; }
    public long? Ukprn { get; set; }
    public DateTime? CreatedDate { get; set; }
    public VacancyStatus Status { get; set; }
    public DateTime? ClosingDate { get; set; }
    public int? Duration { get; set; }
    public DurationUnit? DurationUnit { get; set; }
    public DateTime? ClosedDate { get; set; }
    public ClosureReason? ClosureReason { get; set; }
    public ApplicationMethod? ApplicationMethod { get; set; }
    public string? ProgrammeId { get; set; }
    public DateTime? StartDate { get; set; }
    public string? TrainingTitle { get; set; }
    public long? TransferInfoUkprn { get; set; }
    public string? TransferInfoProviderName { get; set; }
    public TransferReason? TransferInfoReason { get; set; }
    public DateTime? TransferInfoTransferredDate { get; set; }
    public string? TrainingProviderName { get; set; }
    public int NoOfNewApplications { get; set; }
    public int NoOfSuccessfulApplications { get; set; }
    public int NoOfUnsuccessfulApplications { get; set; }
    public int NoOfApplications => NoOfNewApplications + NoOfSuccessfulApplications + NoOfUnsuccessfulApplications;
    public int NoOfSharedApplications { get; set; }
    public int NoOfAllSharedApplications { get; set; }
    public int NoOfEmployerReviewedApplications { get; set; }
    public bool IsTraineeship { get; set; }
    public bool IsTaskListCompleted { get; set; }
    public bool? HasChosenProviderContactDetails { get; set; }
    public ApprenticeshipTypes? ApprenticeshipType { get; set; }
    public SourceOrigin? SourceOrigin { get; set; }
}
