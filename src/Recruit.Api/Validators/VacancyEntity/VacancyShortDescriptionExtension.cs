using FluentValidation;
using Ganss.Xss;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyShortDescriptionExtension
{
    public static void VacancyDescriptionCheck(this IRuleBuilderInitial<Vacancy, string?> rule,
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
            .WithMessage($"What the apprenticeship will do at work contains some invalid characters")
            .WithErrorCode("6")
            .WithState(_ => VacancyRuleSet.Description)
            .ProfanityCheck(profanityListProvider)
            .WithMessage($"What the apprenticeship will do at work must not contain a banned word or phrase")
            .WithErrorCode("609")
            .WithState(_ => VacancyRuleSet.Description)
            .RunCondition(VacancyRuleSet.Description);
    }
    
    public static void VacancyShortDescriptionCheck(this IRuleBuilderInitial<Vacancy, string?> rule,
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