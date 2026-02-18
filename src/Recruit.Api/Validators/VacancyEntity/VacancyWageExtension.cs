using System.IO.Hashing;
using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.InputValidation.Fluent.Extensions;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;
using SFA.DAS.VacancyServices.Wage;

namespace SFA.DAS.Recruit.Api.Validators.VacancyEntity;

public static class VacancyWageExtension
{
    public static void VacancyWageValidation(this IRuleBuilderInitial<Vacancy, Wage?> rule)
    {
        rule
            .NotNull()
            .WithMessage("Select how much you'd like to pay the apprentice")
            .WithErrorCode("46")
            .WithState(_ => VacancyRuleSet.Wage)
            .RunCondition(VacancyRuleSet.Wage);
    }

    public static void VacancyWageFieldsValidation(this IRuleBuilderInitial<Vacancy, Wage?> rule, IHtmlSanitizerService htmlSanitizerService, IProhibitedContentRepository profanityListProvider)
    {
        rule
            .ChildRules(x =>
            {
                x.RuleFor(w => w!.WageType)
                    .NotEmpty()
                    .WithMessage("Select how much the apprentice will be paid")
                    .WithErrorCode("46")
                    .WithState(_ => VacancyRuleSet.Wage)
                    .IsInEnum()
                    .WithMessage("Select how much the apprentice will be paid")
                    .WithErrorCode("46")
                    .WithState(_ => VacancyRuleSet.Wage);
            }).RunCondition(VacancyRuleSet.Wage);
        rule
            .ChildRules(x =>
            {
                x.RuleFor(y => y.WageAdditionalInformation)
                    .MaximumLength(250)
                    .WithMessage("Information about pay must be {MaxLength} characters or less")
                    .WithErrorCode("44")
                    .WithState(_ => VacancyRuleSet.Wage)
                    .ValidHtmlCharacters(htmlSanitizerService)
                    .WithMessage("Information about pay contains some invalid characters")
                    .WithErrorCode("45")
                    .WithState(_ => VacancyRuleSet.Wage)
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage("Information about pay must not contain a banned word or phrase")
                    .WithErrorCode("607")
                    .WithState(_ => VacancyRuleSet.Wage);
            })
                .RunCondition(VacancyRuleSet.Wage);
        rule
            .ChildRules(x =>
            {
                x.RuleFor(w => w!.CompanyBenefitsInformation)
                    .MaximumLength(250)
                    .WithMessage("Company benefits must be {MaxLength} characters or less")
                    .WithErrorCode("44")
                    .WithState(_ => VacancyRuleSet.Wage)
                    .ValidHtmlCharacters(htmlSanitizerService)
                    .WithMessage("Company benefits contains some invalid characters")
                    .WithErrorCode("45")
                    .WithState(_ => VacancyRuleSet.Wage)
                    .ProfanityCheck(profanityListProvider)
                    .WithMessage("Company benefits must not contain a banned word or phrase")
                    .WithErrorCode("607")
                    .WithState(_ => VacancyRuleSet.Wage);
            })
                .RunCondition(VacancyRuleSet.Wage);
    }
    
    public static void VacancyDurationValidation(this IRuleBuilderInitial<Vacancy, Wage?> rule)
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

    public static void VacancyWorkingWeekValidation(this IRuleBuilderInitial<Vacancy, string?> rule, IProhibitedContentRepository profanityListProvider)
    {
        rule
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter details about the working week")
            .WithErrorCode("37")
            .WithState(_ => VacancyRuleSet.WorkingWeekDescription)
            .ValidFreeTextCharacters()
            .WithMessage("Working week details contains some invalid characters")
            .WithErrorCode("38")
            .WithState(_ => VacancyRuleSet.WorkingWeekDescription)
            .MaximumLength(250)
            .WithMessage("Details about the working week must not exceed {MaxLength} characters")
            .WithErrorCode("39")
            .WithState(_ => VacancyRuleSet.WorkingWeekDescription)
            .ProfanityCheck(profanityListProvider)
            .WithMessage("Working week details must not contain a banned word or phrase.")
            .WithErrorCode("606")
            .WithState(_ => VacancyRuleSet.WorkingWeekDescription)
            .RunCondition(VacancyRuleSet.WorkingWeekDescription);
    }

    public static void VacancyWorkingHoursValidation(this IRuleBuilderInitial<Vacancy, decimal?> rule)
    {
        rule
            .NotEmpty()
            .WithMessage($"Enter how many hours the apprenticeship will work each week, including training")
            .WithErrorCode("40")
            .WithState(_ => VacancyRuleSet.WeeklyHours)
            .GreaterThanOrEqualTo(16)
            .WithMessage("The total hours a week must be at least {ComparisonValue}")
            .WithErrorCode("42")
            .WithState(_ => VacancyRuleSet.WeeklyHours)
            .LessThanOrEqualTo(48)
            .WithMessage("The total hours a week must not be more than {ComparisonValue}")
            .WithErrorCode("43")
            .WithState(_ => VacancyRuleSet.WeeklyHours)
            .RunCondition(VacancyRuleSet.WeeklyHours);
    }
    public static IRuleBuilderInitial<Vacancy, Vacancy> FixedWageMustBeGreaterThanApprenticeshipMinimumWage(this IRuleBuilder<Vacancy, Vacancy> ruleBuilder, IMinimumWageProvider minimumWageService)
    {
        return (IRuleBuilderInitial<Vacancy, Vacancy>)ruleBuilder.Custom((vacancy, context) =>
        {
            var wagePeriod = minimumWageService.GetWagePeriod(vacancy.StartDate.Value);

            if (vacancy.Wage.FixedWageYearlyAmount == null || vacancy.Wage.FixedWageYearlyAmount / 52 / vacancy.Wage.WeeklyHours < wagePeriod.ApprenticeshipMinimumWage)
            {
                var minimumYearlyWageForApprentices = WagePresenter.GetDisplayText(VacancyServices.Wage.WageType.ApprenticeshipMinimum, WageUnit.Annually, new WageDetails
                {
                    HoursPerWeek = vacancy.Wage.WeeklyHours,
                    StartDate = vacancy.StartDate.Value
                }).AsWholeMoneyAmount();

                var errorMessage = (vacancy.Status == VacancyStatus.Live) ?
                    $"National Minimum Wage is changing from {wagePeriod.ValidFrom:d MMM yyyy}. So the fixed wage you entered before will no longer be valid. Change the date to before {wagePeriod.ValidFrom:d MMM yyyy} or to change the wage, create a new vacancy" :
                    $"Yearly wage must be at least {minimumYearlyWageForApprentices}";

                var failure = new ValidationFailure(string.Empty, errorMessage)
                {
                    ErrorCode = "49",
                    CustomState = VacancyRuleSet.MinimumWage,
                    PropertyName = $"{nameof(Wage)}.{nameof(Wage.FixedWageYearlyAmount)}"  
                };
                context.AddFailure(failure);
            }
        });
    }
}