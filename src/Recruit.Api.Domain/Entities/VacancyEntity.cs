using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Domain.Entities;

public class VacancyEntity
{
    public Guid Id {get; set;}
    public long? VacancyReference {get; set;}
    public long? AccountId {get; set;}
    public required VacancyStatus Status {get; set;}
    public ApprenticeshipTypes? ApprenticeshipType {get; set;}
    public string? Title {get; set;}
    public OwnerType? OwnerType {get; set;}
    public SourceOrigin? SourceOrigin {get; set;}
    public SourceType? SourceType {get; set;}
    public long? SourceVacancyReference {get; set;}
    public DateTime? ApprovedDate {get; set;}
    public DateTime? CreatedDate {get; set;}
    public DateTime? LastUpdatedDate {get; set;}
    public DateTime? SubmittedDate {get; set;}
    public DateTime? ReviewRequestedDate {get; set;}
    public DateTime? ClosedDate {get; set;}
    public DateTime? DeletedDate {get; set;}
    public DateTime? LiveDate {get; set;}
    public DateTime? StartDate {get; set;}
    public DateTime? ClosingDate {get; set;}
    public int ReviewCount {get; set;}
    public string? ApplicationUrl {get; set;}
    public ApplicationMethod? ApplicationMethod {get; set;}
    public string? ApplicationInstructions {get; set;}
    public string? ShortDescription {get; set;}
    public string? Description {get; set;}
    public string? AnonymousReason {get; set;}
    public bool? DisabilityConfident {get; set;}
    public string? ContactName {get; set;}
    public string? ContactEmail {get; set;}
    public string? ContactPhone {get; set;}
    public string? EmployerDescription {get; set;}
    public string? EmployerLocations {get; set;}
    public AvailableWhere? EmployerLocationOption {get; set;}
    public string? EmployerLocationInformation {get; set;}
    public string? EmployerName {get; set;}
    public EmployerNameOption? EmployerNameOption {get; set;}
    public string? EmployerRejectedReason {get; set;}
    public string? LegalEntityName {get; set;}
    public string? EmployerWebsiteUrl {get; set;}
    public GeoCodeMethod? GeoCodeMethod {get; set;}
    public long? AccountLegalEntityId {get; set;}
    public int? NumberOfPositions {get; set;}
    public string? OutcomeDescription {get; set;}
    public string? ProgrammeId {get; set;}
    public string? Skills {get; set;}
    public string? Qualifications {get; set;}
    public string? ThingsToConsider {get; set;}
    public string? TrainingDescription {get; set;}
    public string? AdditionalTrainingDescription {get; set;}
    public int? Ukprn {get; set;}
    public string? TrainingProvider_Name {get; set;}
    public string? TrainingProvider_Address {get; set;}
    public int? Wage_Duration {get; set;}
    public DurationUnit? Wage_DurationUnit {get; set;}
    public string? Wage_WorkingWeekDescription {get; set;}
    public decimal? Wage_WeeklyHours {get; set;}
    public WageType? Wage_WageType {get; set;}
    public decimal? Wage_FixedWageYearlyAmount {get; set;}
    public string? Wage_WageAdditionalInformation {get; set;}
    public string? Wage_CompanyBenefitsInformation {get; set;}
    public ClosureReason? ClosureReason {get; set;}
    public string? TransferInfo {get; set;}
    public string? AdditionalQuestion1 {get; set;}
    public string? AdditionalQuestion2 {get; set;}
    public bool? HasSubmittedAdditionalQuestions {get; set;}
    public bool? HasChosenProviderContactDetails {get; set;}
    public bool? HasOptedToAddQualifications {get; set;}
    public string? EmployerReviewFieldIndicators {get; set;}
    public string? ProviderReviewFieldIndicators {get; set;}
    public Guid? SubmittedByUserId { get; set; }
    public Guid? ReviewRequestedByUserId { get; set; }
}