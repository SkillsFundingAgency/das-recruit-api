using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.Validators;

public static class VacancyRuleSetExtensions
{
    internal static void RunCondition<T>(this IRuleBuilderOptions<Domain.Entities.VacancyEntity, T> context, VacancyEntityRuleSet condition)
    {
        context.Configure(c=>c.ApplyCondition(x => x.CanRunValidator(condition)));
    }
    private static bool CanRunValidator<T>(this ValidationContext<T> context, VacancyEntityRuleSet validationToCheck)
    {
        var validationsToRun = (VacancyEntityRuleSet)context.RootContextData[ValidationConstants.ValidationsRulesKey];

        return (validationsToRun & validationToCheck) > 0;
    }
    public static IRuleBuilderOptions<T, string> ProfanityCheck<T>(this IRuleBuilder<T, string> rule, IProhibitedContentRepository profanityListProvider)
    {
        return rule.SetValidator(new ProfanityCheckValidator<T, string>(profanityListProvider));
    }
}