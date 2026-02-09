using FluentValidation;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyStartDateExtension
{
    public static void VacancyStartDateCheck(this IRuleBuilderInitial<Vacancy, DateTime?> rule)
    {
        rule
            .NotNull()
            .WithMessage("Enter when you expect the apprenticeship to start")
            .WithErrorCode("20")
            .WithState(_ => VacancyRuleSet.StartDate)
            .GreaterThan(v => v.ClosingDate)
            .WithMessage("Start date cannot be before the closing date. We advise using a date more than 14 days from now.")
            .WithErrorCode("22")
            .WithState(_ => VacancyRuleSet.StartDate)
            .RunCondition(VacancyRuleSet.StartDate);
    }
}