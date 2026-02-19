using FluentValidation;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancySkillsExtension
{
    public static void VacancySkillsValidation(this IRuleBuilderInitial<PutVacancyRequest, List<string>?> rule, 
        IProhibitedContentRepository profanityListProvider)
    {
        rule
            .Must(s => s is { Count: > 0 })
            .WithMessage("Select the skills and personal qualities you'd like the applicant to have")
            .WithErrorCode("51")
            .WithState(_ => VacancyRuleSet.Skills)
            .RunCondition(VacancyRuleSet.Skills);

        rule.ChildRules(x =>
        {
            x.RuleForEach(x => x)
                .NotEmpty()
                .WithMessage("You must include a skill or quality")
                .WithErrorCode("99")
                .WithState(_ => VacancyRuleSet.Skills)
                .ValidFreeTextCharacters()
                .WithMessage("Skill contains some invalid characters")
                .WithErrorCode("6")
                .WithState(_ => VacancyRuleSet.Skills)
                .MaximumLength(30)
                .WithMessage("Skill or quality must not exceed {MaxLength} characters")
                .WithErrorCode("7")
                .WithState(_ => VacancyRuleSet.Skills)
                .ProfanityCheck(profanityListProvider)
                .WithMessage("Skill or quality must not contain a banned word or phrase.")
                .WithErrorCode("608")
                .WithState(_ => VacancyRuleSet.Skills);
        }).RunCondition(VacancyRuleSet.Skills);;

    }
}