using System.IO.Hashing;
using FluentValidation;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyDurationExtension
{
    public static void VacancyDurationCheck(this IRuleBuilderInitial<Vacancy, Wage?> rule)
    {
        const int minimumVacancyDurationInMonths = 8;
        
        rule
            .ChildRules(x =>
            {
                x.RuleFor(w => w!.Duration)
                    .NotEmpty()
                    .WithErrorCode("34")
                    .WithState(_ => VacancyRuleSet.Duration)
                    .WithMessage("Enter how long the whole apprenticeship is, including work and training")
                    .Must((wage, value) =>
                    {
                        if (wage!.DurationUnit == DurationUnit.Month && value < minimumVacancyDurationInMonths)
                        {
                            return false;
                        }
                        return true;
                    })
                    .WithErrorCode("36")
                    .WithState(_ => VacancyRuleSet.Duration)
                    .WithMessage($"Expected duration must be at least {minimumVacancyDurationInMonths} months");
                x.RuleFor(w => w!.DurationUnit)
                    .NotEmpty()
                    .WithErrorCode("34")
                    .WithMessage("Enter how long the whole apprenticeship is, including work and training")
                    .WithState(_ => VacancyRuleSet.Duration)
                    .IsInEnum()
                    .WithMessage("Enter how long the whole apprenticeship is, including work and training")
                    .WithErrorCode("34")
                    .WithState(_ => VacancyRuleSet.Duration);
            })
            .ChildRules(x =>
            {
                x.RuleFor(w => w!.Duration)
                    .Must((wage, value) =>
                    {
                        if (((wage!.DurationUnit != DurationUnit.Month || !(value >= minimumVacancyDurationInMonths))
                             && (wage!.DurationUnit != DurationUnit.Year || !(value >= 1)))
                            || wage!.WeeklyHours is not < 30m)
                        {
                            return true;
                        }

                        var numberOfMonths = (int)Math.Ceiling(30 / wage.WeeklyHours.GetValueOrDefault() * 8);

                        if (wage.DurationUnit == DurationUnit.Year)
                        {
                            wage.Duration *= minimumVacancyDurationInMonths;
                        }

                        if (numberOfMonths > wage!.Duration)
                        {
                            return false;
                        }

                        return true;
                    })
                    .WithMessage(wage =>
                    {
                        int numberOfMonths = (int)Math.Ceiling(30 / wage!.WeeklyHours.GetValueOrDefault() *
                                                               minimumVacancyDurationInMonths);
                        return
                            $"Duration of apprenticeship must be {numberOfMonths} months based on the number of hours per week entered";
                    })
                    .WithErrorCode("36")
                    .WithState(_ => VacancyRuleSet.Duration);
            })
            .WithErrorCode("36")
            .WithState(_ => VacancyRuleSet.Duration)
            .RunCondition(VacancyRuleSet.Duration);
    }
}