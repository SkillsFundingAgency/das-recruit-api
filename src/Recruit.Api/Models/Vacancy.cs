using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Models;

public class Vacancy
{
    public Guid Id { get; init; }
    public long? VacancyReference { get; init; }
    public long? AccountId { get; init; }
    public required VacancyStatus Status { get; init; }
    public ApprenticeshipTypes? ApprenticeshipType { get; init; }
    public string? Title { get; init; }
    public OwnerType? OwnerType { get; init; }
    public SourceOrigin? SourceOrigin { get; init; }
    public SourceType? SourceType { get; init; }
    public long? SourceVacancyReference { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
    public DateTime? SubmittedDate { get; init; }
    public DateTime? ReviewDate { get; init; }
    public DateTime? ClosedDate { get; init; }
    public DateTime? DeletedDate { get; init; }
    public DateTime? LiveDate { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? ClosingDate { get; init; }
    public int ReviewCount { get; init; }
    public string? ApplicationUrl { get; init; }
    public ApplicationMethod? ApplicationMethod { get; init; }
    public string? ApplicationInstructions { get; init; }
    public string? ShortDescription { get; init; }
    public string? Description { get; init; }
    public string? AnonymousReason { get; init; }
    public bool? DisabilityConfident { get; init; }
    public ContactDetail? Contact { get; set; }
    public string? EmployerDescription { get; init; }
    public List<Address>? EmployerLocations { get; set; }
    public AvailableWhere? EmployerLocationOption { get; init; }
    public string? EmployerLocationInformation { get; init; }
    public string? EmployerName { get; init; }
    public EmployerNameOption? EmployerNameOption { get; init; }
    public string? EmployerRejectedReason { get; init; }
    public string? LegalEntityName { get; init; }
    public string? EmployerWebsiteUrl { get; init; }
    public GeoCodeMethod? GeoCodeMethod { get; init; }
    public long? AccountLegalEntityId { get; init; }
    public int? NumberOfPositions { get; init; }
    public string? OutcomeDescription { get; init; }
    public string? ProgrammeId { get; init; }
    public List<string>? Skills { get; init; }
    public List<Qualification>? Qualifications { get; set; }
    public string? ThingsToConsider { get; init; }
    public string? TrainingDescription { get; init; }
    public string? AdditionalTrainingDescription { get; init; }
    public TrainingProvider? TrainingProvider { get; init; }
    public Wage? Wage { get; set; }
    public ClosureReason? ClosureReason { get; init; }
    public TransferInfo? TransferInfo { get; init; }
    public string? AdditionalQuestion1 { get; init; }
    public string? AdditionalQuestion2 { get; init; }
    public bool? HasSubmittedAdditionalQuestions { get; init; }
    public bool? HasChosenProviderContactDetails { get; init; }
    public bool? HasOptedToAddQualifications { get; init; }
    public List<ReviewFieldIndicator>? EmployerReviewFieldIndicators { get; init; }
    public List<ReviewFieldIndicator>? ProviderReviewFieldIndicators { get; init; }
    public string? SubmittedByUserId { get; init; }
}

public class Address
{
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? AddressLine4 { get; set; }
    public required string Postcode { get; set; }
    //public string? Country { get; set; } // TODO: do we need this now we're England only?
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class ContactDetail
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}

public class ReviewFieldIndicator
{
    public required string FieldIdentifier { get; set; }
    public bool IsChangeRequested { get; set; }
}

public class TrainingProvider
{
    public long? Ukprn { get; set; }
    public required string Name { get; set; }
    public required Address Address { get; set; }
}

public class TransferInfo
{
    public long Ukprn { get; set; }
    public string ProviderName { get; set; }
    public string LegalEntityName { get; set; }
    public DateTime TransferredDate { get; set; }
    public TransferReason Reason { get; set; }
}

public class Qualification
{
    public string QualificationType { get; set; }
    public string Subject { get; set; }
    public string Grade { get; set; }
    public int? Level { get; set; }
    public QualificationWeighting? Weighting { get; set; }
    public string OtherQualificationName { get; set; }
}

public class Wage
{
    public int? Duration { get; set; }
    public DurationUnit? DurationUnit { get; set; }
    public string? WorkingWeekDescription { get; set; }
    public decimal? WeeklyHours { get; set; }
    public WageType? WageType { get; set; }
    public decimal? FixedWageYearlyAmount { get; set; }
    public string? WageAdditionalInformation { get; set; }
    public string? CompanyBenefitsInformation { get; set; }
}