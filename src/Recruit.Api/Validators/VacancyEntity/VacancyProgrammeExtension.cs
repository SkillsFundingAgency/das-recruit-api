using FluentValidation;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyProgrammeExtension
{
    public static void VacancyProgrammeIdCheck(this IRuleBuilderInitial<Vacancy, string?> rule)
    {
        rule
            .NotEmpty()
            .WithMessage("Enter a valid training course")
            .WithErrorCode("25")
            .WithState(_ => VacancyRuleSet.TrainingProgramme)
            .RunCondition(VacancyRuleSet.TrainingProgramme);;
    }
}