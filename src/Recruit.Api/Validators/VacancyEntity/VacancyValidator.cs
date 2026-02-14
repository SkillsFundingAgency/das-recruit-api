using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public class VacancyValidator : AbstractValidator<Vacancy>
{
    public VacancyValidator(IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizerService, TimeProvider timeProvider, IExternalWebsiteHealthCheckService externalWebsiteHealthCheckService)
    {
        ValidateVacancy(profanityListProvider, htmlSanitizerService, timeProvider, externalWebsiteHealthCheckService);
    }
    
    private void ValidateVacancy(IProhibitedContentRepository profanityListProvider,
        IHtmlSanitizerService htmlSanitizerService, TimeProvider timeProvider, IExternalWebsiteHealthCheckService externalWebsiteHealthCheckService)
    {
        RuleFor(x => x.Title).VacancyTitleCheck(profanityListProvider);
        RuleFor(x => x.Wage).VacancyWageCheck();
        When(x => x.Wage != null, () =>
        {
            RuleFor(x => x.Wage).VacancyDurationCheck();
            RuleFor(x => x.Wage!.WorkingWeekDescription).VacancyWorkingWeekCheck(profanityListProvider);
            RuleFor(x => x.Wage!.WeeklyHours).VacancyWorkingHoursCheck();
            RuleFor(x=>x.Wage).VacancyWageFieldsCheck(htmlSanitizerService, profanityListProvider);
        });
        RuleFor(x => x).ValidateOrganisationCheck(profanityListProvider);
        RuleFor(x => x.NumberOfPositions).VacancyNumberOfPositionsCheck();
        RuleFor(x => x.ShortDescription).VacancyShortDescriptionCheck(profanityListProvider, htmlSanitizerService);
        RuleFor(x => x.ClosingDate).VacancyClosingDateCheck(timeProvider);
        RuleFor(x => x.StartDate).VacancyStartDateCheck();
        RuleFor(x => x.ProgrammeId).VacancyProgrammeIdCheck();
        When(x=> x.ApprenticeshipType != ApprenticeshipTypes.Foundation,()=>
        {
            RuleFor(x => x.Skills).VacancySkillsCheck(profanityListProvider);
        });
        When(x => x.ApprenticeshipType is ApprenticeshipTypes.Standard or null && x.HasOptedToAddQualifications is true, () =>
        {
            RuleFor(x => x.Qualifications).VacancyQualificationCheck(profanityListProvider);
        });
        RuleFor(x => x.Description).VacancyDescriptionCheck(profanityListProvider, htmlSanitizerService);
        When(x => !string.IsNullOrEmpty(x.AdditionalQuestion1) && x.ApplicationMethod != ApplicationMethod.ThroughExternalApplicationSite, () =>
        {
            RuleFor(x=>x.AdditionalQuestion1)!.ValidateAdditionalQuestionCheck(profanityListProvider, VacancyRuleSet.AdditionalQuestion1);
        });
        When(x => !string.IsNullOrEmpty(x.AdditionalQuestion2) && x.ApplicationMethod != ApplicationMethod.ThroughExternalApplicationSite, () =>
        {
            RuleFor(x=>x.AdditionalQuestion2)!.ValidateAdditionalQuestionCheck(profanityListProvider, VacancyRuleSet.AdditionalQuestion2);
        });
        When(x => !string.IsNullOrEmpty(x.TrainingDescription), () =>
        {
            RuleFor(x=>x.TrainingDescription).VacancyTrainingDescriptionCheck(profanityListProvider, htmlSanitizerService);
        });
        When(x => !string.IsNullOrEmpty(x.AdditionalTrainingDescription), () =>
        {
            RuleFor(x=>x.AdditionalTrainingDescription).VacancyAdditionalTrainingInformationCheck(profanityListProvider, htmlSanitizerService);
        });
        RuleFor(x=>x.OutcomeDescription).VacancyOutcomeDescriptionCheck(profanityListProvider, htmlSanitizerService);
        RuleFor(x => x).ValidationApplicationMethod(profanityListProvider, externalWebsiteHealthCheckService);
        When(x => x.Contact != null, () =>
        {
            RuleFor(x => x.Contact).CheckContactDetail(profanityListProvider);    
        });
        
    }
}

[Flags]
public enum VacancyRuleSet : long
{
    None = 0L,
    EmployerName = 1L,
    EmployerAddress = 1L << 1,
    NumberOfPositions = 1L << 2,
    ShortDescription = 1L << 3,
    Title = 1L << 4,
    ClosingDate = 1L << 5,
    StartDate = 1L << 6,
    TrainingProgramme = 1L << 7,
    Duration = 1L << 8,
    WorkingWeekDescription = 1L << 9,
    WeeklyHours = 1L << 10,
    Wage = 1L << 11,
    StartDateEndDate = 1L << 12,
    MinimumWage = 1L << 13,
    TrainingExpiryDate = 1L << 14,
    Skills = 1L << 15,
    Qualifications = 1L << 16,
    Description = 1L << 17,
    TrainingDescription = 1L << 18,
    OutcomeDescription = 1L << 19,
    ApplicationMethod = 1L << 20,
    EmployerContactDetails = 1L << 21,
    ProviderContactDetails = 1L << 22,
    ThingsToConsider = 1L << 23,
    EmployerDescription = 1L << 24,
    EmployerWebsiteUrl = 1L << 25,
    TrainingProvider = 1L << 26,
    EmployerNameOption = 1L << 27,
    LegalEntityName = 1L << 28,
    TradingName = 1L << 29,
    WorkExperience = 1L << 30,
    RouteId = 1L << 31,
    AdditionalQuestion1 = 1L << 32,
    AdditionalQuestion2 = 1L << 33,
    CompetitiveWage = 1L << 34,
    AdditionalTrainingDescription = 1L << 35,
    TrainingProviderDeliverCourse = 1L << 36,
    All = ~None,
}