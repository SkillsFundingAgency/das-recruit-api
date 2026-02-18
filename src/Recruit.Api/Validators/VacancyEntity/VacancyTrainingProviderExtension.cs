using FluentValidation;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyTrainingProviderExtension
{
    public static void VacancyTrainingProviderValidation(this IRuleBuilderInitial<Vacancy, TrainingProvider> rule)
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

            //TODO Needs FAI-2972 to be implemented
            // x.When(tp => tp.Ukprn.ToString().Length == 8, () =>
            // {
            //     x.RuleFor(tp => tp)
            //         .TrainingProviderMustNotBeBlocked(blockedOrganisationRepo);
            // });
        }).WithState(_ => VacancyRuleSet.TrainingProvider).RunCondition(VacancyRuleSet.TrainingProvider);
        
    }
}