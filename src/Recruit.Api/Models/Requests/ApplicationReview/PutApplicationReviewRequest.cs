using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models.Requests.ApplicationReview;

public class PutApplicationReviewRequest
{
    public DateTime? SubmittedDate { get; init; }
    public DateTime? WithdrawnDate { get; set; }
    public DateTime? DateSharedWithEmployer { get; init; }
    public DateTime? ReviewedDate { get; init; }
    public DateTime? StatusUpdatedDate { get; init; }
    public Guid CandidateId { get; init; }
    public Guid? ApplicationId { get; init; }
    public Guid? LegacyApplicationId { get; init; }
    public bool HasEverBeenEmployerInterviewing { get; init; }
    public int Ukprn { get; set; }
    public long AccountId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public long VacancyReference { get; init; }
    public ApplicationReviewStatus Status { get; set; }
    public string? TemporaryReviewStatus { get; init; }
    public string VacancyTitle { get; init; }
    public string? AdditionalQuestion1 { get; init; }
    public string? AdditionalQuestion2 { get; init; }
    public string? CandidateFeedback { get; init; }
    public string? EmployerFeedback { get; init; }
}