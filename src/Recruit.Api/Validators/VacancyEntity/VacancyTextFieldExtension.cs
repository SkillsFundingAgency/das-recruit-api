using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

//TODO look at combining these as they are validating the same thing
public static class VacancyTextFieldExtension
{
    public static void VacancyDescriptionValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule.NotEmpty()
            .WithMessage($"Enter what the apprenticeship will do at work")
            .WithErrorCode("53")
            .WithState(_ => VacancyRuleSet.Description)
            .MaximumLength(4000)
            .WithMessage($"What the apprenticeship will do at work must not exceed {{MaxLength}} characters")
            .WithErrorCode("7")
            .WithState(_ => VacancyRuleSet.Description)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("What the apprenticeship will do at work contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_ => VacancyRuleSet.Description)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("What the apprenticeship will do at work must not contain a banned word or phrase")
            .WithErrorCode("609")
            .WithState(_ => VacancyRuleSet.Description)
            .RunCondition(VacancyRuleSet.Description);
    }

    public static void VacancyTrainingDescriptionValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule
            .MaximumLength(4000)
            .WithMessage("The apprentice’s training plan must not exceed 4000 characters")
            .WithErrorCode("321")
            .WithState(_ => VacancyRuleSet.TrainingDescription)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("The apprentice’s training plan must not contain a restricted word")
            .WithErrorCode("322")
            .WithState(_ => VacancyRuleSet.TrainingDescription)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("The apprentice’s training plan contains some invalid characters")
            .WithErrorCode("346")
            .WithState(_ => VacancyRuleSet.TrainingDescription)
            .RunCondition(VacancyRuleSet.TrainingDescription);
    }

    public static void VacancyThingsToConsiderValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule
            .MaximumLength(4000)
            .WithMessage("Other requirements must not exceed {MaxLength} characters")
            .WithErrorCode("75")
            .WithState(_ => VacancyRuleSet.ThingsToConsider)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("Other requirements contains some invalid characters")
            .WithErrorCode("76")
            .WithState(_ => VacancyRuleSet.ThingsToConsider)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Other requirements must not contain a banned word or phrase.")
            .WithErrorCode("613")
            .WithState(_ => VacancyRuleSet.ThingsToConsider)
            .RunCondition(VacancyRuleSet.ThingsToConsider);
    }

    public static void VacancyEmployerInformationValidation(this IRuleBuilderInitial<Vacancy, string?> rule, IHtmlSanitizerService htmlSanitizer, IProhibitedContentRepository profanityListProvider)
    {
        rule.NotEmpty()
            .WithMessage("Enter details about the employer")
            .WithErrorCode("80")
            .WithState(_ => VacancyRuleSet.EmployerDescription)
            .MaximumLength(4000)
            .WithMessage("Information about the employer must not exceed {MaxLength} characters")
            .WithErrorCode("77")
            .WithState(_ => VacancyRuleSet.EmployerDescription)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("Information about the employer contains some invalid characters")
            .WithErrorCode("78")
            .WithState(_ => VacancyRuleSet.EmployerDescription)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Information about the employer must not contain a banned word or phrase.")
            .WithErrorCode("614")
            .WithState(_ => VacancyRuleSet.EmployerDescription)
            .RunCondition(VacancyRuleSet.EmployerDescription);
    }

    public static void VacancyAdditionalTrainingInformationValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule
            .MaximumLength(4000)
            .WithMessage("Any additional training information must not exceed 4000 characters")
            .WithErrorCode("341")
            .WithState(_ => VacancyRuleSet.AdditionalTrainingDescription)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Any additional training information must not contain a restricted word")
            .WithErrorCode("342")
            .WithState(_ => VacancyRuleSet.AdditionalTrainingDescription)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("Any additional training information contains some invalid characters")
            .WithErrorCode("344")
            .WithState(_ => VacancyRuleSet.AdditionalTrainingDescription)
            .RunCondition(VacancyRuleSet.AdditionalTrainingDescription);
    }
    
    public static void VacancyOutcomeDescriptionValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule
            .NotEmpty()
            .WithMessage($"Enter the expected career progression after this apprenticeship")
            .WithErrorCode("55")
            .WithState(_ => VacancyRuleSet.OutcomeDescription)
            .MaximumLength(4000)
            .WithMessage("Expected career progression must not exceed {MaxLength} characters")
            .WithErrorCode("7")
            .WithState(_ => VacancyRuleSet.OutcomeDescription)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("What is the expected career progression after this apprenticeship description contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_ => VacancyRuleSet.OutcomeDescription)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("What is the expected career progression after this apprenticeship description must not contain a banned word or phrase.")
            .WithErrorCode("611")
            .WithState(_ => VacancyRuleSet.OutcomeDescription)
            .RunCondition(VacancyRuleSet.OutcomeDescription);
    }
    
    public static void VacancyShortDescriptionValidation(this IRuleBuilderInitial<Vacancy, string?> rule,
        IProhibitedContentRepository profanityListProvider, IHtmlSanitizerService htmlSanitizer)
    {
        rule
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter a short description of the apprenticeship")
            .WithErrorCode("12")
            .WithState(_ => VacancyRuleSet.ShortDescription)
            .MaximumLength(350)
            .WithMessage("Summary of the apprenticeship must not exceed {{MaxLength}} characters")
            .WithErrorCode("13")
            .WithState(_ => VacancyRuleSet.ShortDescription)
            .MinimumLength(50)
            .WithMessage("Summary of the apprenticeship must be at least {{MinLength}} characters")
            .WithErrorCode("14")
            .WithState(_ => VacancyRuleSet.ShortDescription)
            .ValidHtmlCharacters(htmlSanitizer)
            .WithMessage("Short description of the apprenticeship contains some invalid characters")
            .WithErrorCode("15")
            .WithState(_ => VacancyRuleSet.ShortDescription)!
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Short description of the apprenticeship must not contain a banned word or phrase.")
            .WithErrorCode("605")
            .WithState(_ => VacancyRuleSet.ShortDescription)
            .RunCondition(VacancyRuleSet.ShortDescription);
    }
}