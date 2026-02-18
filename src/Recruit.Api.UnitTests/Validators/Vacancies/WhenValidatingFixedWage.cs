using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingFixedWage : VacancyValidationTestsBase
{
    [TestCase(3.70f, 40, 12000.00f)] // £5.77 per hour
    public void FixedWageAmountIsValidIfOverMinimumWage(float minimumWageTestValue, int hoursPerWeekValue, float yearlyWageAmcountValue)
    {
        var startDate = DateTime.UtcNow.Date;
        var minimumWageAmount = Convert.ToDecimal(minimumWageTestValue);

        var vacancy = new Vacancy
        {
            StartDate = startDate,
            Wage = new Wage
            {
                WageType = WageType.FixedWage,
                FixedWageYearlyAmount = Convert.ToDecimal(yearlyWageAmcountValue),
                WeeklyHours = hoursPerWeekValue
            },
            Status = VacancyStatus.Draft
        };

        MinimumWageProvider.Setup(x => 
                 x.GetWagePeriod(It.IsAny<DateTime>())).Returns(new MinimumWage{ApprenticeshipMinimumWage = minimumWageAmount});

        var result = Validator.Validate(vacancy, VacancyRuleSet.MinimumWage);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase(3.70f, 40, "7000.00")] // £3.37 per hour
    [TestCase(3.70f, 40, null)]
    public void FixedWageAmountIsNotValidIfUnderMinimumWage(float minimumWageTestValue, int hoursPerWeekValue, string? yearlyWageAmcountValue)
    {
        var startDate = DateTime.UtcNow.Date;
        var minimumWageAmount = Convert.ToDecimal(minimumWageTestValue);

        var vacancy = new Vacancy
        {
            StartDate = startDate,
            Wage = new Wage
            {
                WageType = WageType.FixedWage,
                FixedWageYearlyAmount = yearlyWageAmcountValue != null ? Convert.ToDecimal(yearlyWageAmcountValue) : default(decimal?),
                WeeklyHours = hoursPerWeekValue
            },
            Status = VacancyStatus.Draft
        };

        MinimumWageProvider.Setup(x =>
            x.GetWagePeriod(It.IsAny<DateTime>())).Returns(new MinimumWage { ApprenticeshipMinimumWage = minimumWageAmount });

        var result = Validator.Validate(vacancy, VacancyRuleSet.MinimumWage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(Wage)}.{nameof(Wage.FixedWageYearlyAmount)}" );
        result.Errors[0].ErrorCode.Should().Be("49");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.MinimumWage);
    }
}