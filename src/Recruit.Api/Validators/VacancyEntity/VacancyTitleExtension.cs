using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyTitleExtension
{
    public static void VacancyTitleValidation(this IRuleBuilderInitial<PutVacancyRequest, string?> rule,
        IProhibitedContentRepository profanityListProvider)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter a title for this apprenticeship")
            .WithErrorCode("1")
            .WithState(_ => VacancyRuleSet.Title)
            .MaximumLength(100)
            .WithMessage("Title must not exceed {MaxLength} characters")
            .WithErrorCode("2")
            .WithState(_ => VacancyRuleSet.Title)
            .ValidFreeTextCharacters()
            .WithMessage("Title contains some invalid characters")
            .WithErrorCode("3")
            .WithState(_ => VacancyRuleSet.Title)
            .Matches(ValidationConstants.ContainsApprenticeOrApprenticeshipRegex)
            .WithMessage("Enter a title which includes the word 'apprentice' or 'apprenticeship'")
            .WithErrorCode("200")
            .WithState(_ => VacancyRuleSet.Title)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Title must not contain a banned word or phrase.")
            .WithErrorCode("601")
            .WithState(_ => VacancyRuleSet.Title)
            .RunCondition(VacancyRuleSet.Title);
    }
}