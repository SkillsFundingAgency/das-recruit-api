using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyStartDateExtension
{
    public static void VacancyStartDateValidation(this IRuleBuilderInitial<Vacancy, DateTime?> rule)
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
    
    public static IRuleBuilderInitial<Vacancy, Vacancy> ClosingDateMustBeLessThanStartDate(this IRuleBuilder<Vacancy, Vacancy> ruleBuilder)
    {
        return (IRuleBuilderInitial<Vacancy, Vacancy>)ruleBuilder.Custom((vacancy, context) =>
        {
            if (vacancy.StartDate.Value.Date <= vacancy.ClosingDate.Value.Date)
            {
                var failure = new ValidationFailure(string.Empty, "The possible start date should be after the closing date")
                {
                    ErrorCode = "24",
                    CustomState = VacancyRuleSet.StartDateEndDate
                };
                context.AddFailure(failure);
            }
        });
    }
}