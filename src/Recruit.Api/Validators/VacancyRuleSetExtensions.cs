using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.Validators;

public static class VacancyRuleSetExtensions
{
    internal static void RunCondition<T>(this IRuleBuilderOptions<PutVacancyRequest, T> context, VacancyRuleSet condition)
    {
        context.Configure(c=>c.ApplyCondition(x => x.CanRunValidator(condition)));
    }
    private static bool CanRunValidator<T>(this ValidationContext<T> context, VacancyRuleSet validationToCheck)
    {
        var validationsToRun = (VacancyRuleSet)context.RootContextData[ValidationConstants.ValidationsRulesKey];

        return (validationsToRun & validationToCheck) > 0;
    }
    public static IRuleBuilderOptions<T, string> ProfanityCheck<T>(this IRuleBuilder<T, string> rule, IProhibitedContentRepository profanityListProvider)
    {
        return rule.SetValidator(new ProfanityCheckValidator<T, string>(profanityListProvider));
    }
    
    internal static IRuleBuilderInitial<PutVacancyRequest, T> RunCondition<T>(this IRuleBuilderInitial<PutVacancyRequest, T> context, VacancyRuleSet condition)
    {
        return context.Configure(c=>c.ApplyCondition(x => x.CanRunValidator(condition)));
    }
}