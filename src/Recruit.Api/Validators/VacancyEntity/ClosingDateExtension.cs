using FluentValidation;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class ClosingDateExtension
{
    public static void VacancyClosingDateCheck(this IRuleBuilderInitial<VacancyRequest, DateTime?> rule, TimeProvider timeProvider)
    {
        rule
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Enter an application closing date")
            .WithErrorCode("16")
            .WithState(_ => VacancyRuleSet.ClosingDate)
            .Must((vacancy, closingDate) =>
            {
                var closing = closingDate!.Value.Date;
                var today = timeProvider.GetUtcNow().Date;

                if (vacancy.CanExtendStartAndClosingDates())
                {
                    return closing >= today;
                }
                return closing >= today.AddDays(7);
            })
            .WithMessage(vacancy =>
                (vacancy.CanExtendStartAndClosingDates())
                    ? "Closing date cannot be in the past."
                    : "Closing date should be at least 7 days in the future.")
            .WithErrorCode("18")
            .WithState(_ => VacancyRuleSet.ClosingDate)
            .RunCondition(VacancyRuleSet.ClosingDate);
    }
    private static bool CanExtendStartAndClosingDates(this VacancyRequest vacancy)
    {
        return vacancy.Status == VacancyStatus.Live && vacancy.DeletedDate == null;
    }
}