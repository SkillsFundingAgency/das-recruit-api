using FluentValidation;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyProgrammeExtension
{
    public static void VacancyProgrammeIdValidation(this IRuleBuilderInitial<VacancyRequest, string?> rule)
    {
        rule
            .NotEmpty()
            .WithMessage("Enter a valid training course")
            .WithErrorCode("25")
            .WithState(_ => VacancyRuleSet.TrainingProgramme)
            .RunCondition(VacancyRuleSet.TrainingProgramme);;
    }
}