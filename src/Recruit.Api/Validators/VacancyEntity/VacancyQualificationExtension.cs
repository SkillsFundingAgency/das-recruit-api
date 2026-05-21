using FluentValidation;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyQualificationExtension
{
    public static void VacancyQualificationValidation(this IRuleBuilderInitial<VacancyRequest, List<Qualification>?> rule, IProhibitedContentRepository profanityListProvider)
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
                .SetValidator(new VacancyQualificationValidation(profanityListProvider))
                .WithState(_ => VacancyRuleSet.Qualifications);
        }).RunCondition(VacancyRuleSet.Qualifications);
        
    }
    
}