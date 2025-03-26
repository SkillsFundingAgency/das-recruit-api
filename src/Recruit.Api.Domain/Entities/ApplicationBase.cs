namespace Recruit.Api.Domain.Entities
{
    public abstract record ApplicationBase
    {
        public bool HasEverBeenEmployerInterviewing { get; init; }
        public DateTime CreatedDate { get; set; }
        public DateTime SubmittedDate { get; init; }
        public DateTime WithdrawnDate { get; set; }
        public DateTime? DateSharedWithEmployer { get; init; }
        public DateTime? ReviewedDate { get; init; }
        public DateTime? StatusUpdatedDate { get; init; }
        public Guid CandidateId { get; init; }
        public Guid Id { get; set; }
        public Guid? ApplicationId { get; init; }
        public Guid? LegacyApplicationId { get; init; }
        public int Ukprn { get; set; }
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long VacancyReference { get; init; }
        public string VacancyTitle { get; init; }
        public short Owner { get; init; }
        public string? AdditionalQuestion1 { get; init; }
        public string? AdditionalQuestion2 { get; init; }
        public string? CandidateFeedback { get; init; }
        public string? EmployerFeedback { get; init; }
        public string Status { get; init; }
    }
}
