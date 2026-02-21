using FluentValidation;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyNumberOfPositionsExtension
{
    public static void VacancyNumberOfPositionsValidation(this IRuleBuilderInitial<VacancyRequest, int?> rule)
    {
        rule
            .Must(x => x is > 0)
            .WithMessage("Enter the number of positions for this apprenticeship")
            .WithErrorCode("10")
            .WithState(_ => VacancyRuleSet.NumberOfPositions)
            .RunCondition(VacancyRuleSet.NumberOfPositions);
    }
}