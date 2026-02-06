using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyTitleValidatorExtension
{
    public static void VacancyTitleCheck(this IRuleBuilderInitial<Domain.Entities.VacancyEntity, string?> rule,
        IProhibitedContentRepository profanityListProvider)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter a title for this apprenticeship")
            .WithErrorCode("1")
            .WithState(_ => VacancyEntityRuleSet.Title)
            .MaximumLength(100)
            .WithMessage("Title must not exceed {MaxLength} characters")
            .WithErrorCode("2")
            .WithState(_ => VacancyEntityRuleSet.Title)
            .ValidFreeTextCharacters()
            .WithMessage("Title contains some invalid characters")
            .WithErrorCode("3")
            .WithState(_ => VacancyEntityRuleSet.Title)
            .Matches(ValidationConstants.ContainsApprenticeOrApprenticeshipRegex)
            .WithMessage("Enter a title which includes the word 'apprentice' or 'apprenticeship'")
            .WithErrorCode("200")
            .WithState(_ => VacancyEntityRuleSet.Title)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Title must not contain a banned word or phrase.")
            .WithErrorCode("601")
            .WithState(_ => VacancyEntityRuleSet.Title)
            .RunCondition(VacancyEntityRuleSet.Title);
    }
}