using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyQualificationExtension
{
    public static void VacancyQualificationCheck(this IRuleBuilderInitial<Vacancy, List<Qualification>?> rule, IProhibitedContentRepository profanityListProvider)
    {
        rule
            .Must(q => q is { Count: > 0 })
            .WithMessage("You must add a qualification")
            .WithErrorCode("52")
            .WithState(_ => VacancyRuleSet.Qualifications)
            .RunCondition(VacancyRuleSet.Qualifications);

        rule.ChildRules(x =>
        {
            x.RuleForEach(q => q)
                .NotEmpty()
                .SetValidator(new VacancyQualificationValidator(profanityListProvider))
                .WithState(_ => VacancyRuleSet.Qualifications);
        }).RunCondition(VacancyRuleSet.Qualifications);
        
    }
    
}