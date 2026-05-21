using FluentValidation;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyTrainingProviderExtension
{
    public static void VacancyTrainingProviderValidation(this IRuleBuilderInitial<VacancyRequest, TrainingProvider> rule)
    {
        rule.Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("You must enter a training provider")
            .WithErrorCode("101")
            .WithState(_ => VacancyRuleSet.TrainingProvider)
            .ChildRules(x=>{
            x.RuleFor(tp => tp.Ukprn.ToString())
                .NotEmpty()
                .WithMessage("You must enter a training provider")
                .WithErrorCode("101")
                .WithState(_=>VacancyRuleSet.TrainingProvider)
                .Length(8)
                .WithMessage("The UKPRN is 8 digits")
                .WithErrorCode("99")
                .WithState(_ => VacancyRuleSet.TrainingProvider);
        }).WithState(_ => VacancyRuleSet.TrainingProvider).RunCondition(VacancyRuleSet.TrainingProvider);
    }
}