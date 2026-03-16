using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingWageWeeklyHours : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenWeeklyHoursIsValid()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WeeklyHours = 30
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.WeeklyHours);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void WeeklyHoursMustHaveAValue()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WeeklyHours = null
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.WeeklyHours);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WeeklyHours)}");
        result.Errors[0].ErrorCode.Should().Be("40");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.WeeklyHours);
    }

    [Test]
    public void WeeklyHoursMustBeMoreThan16()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WeeklyHours = 15
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.WeeklyHours);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WeeklyHours)}");
        result.Errors[0].ErrorCode.Should().Be("42");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.WeeklyHours);
    }

    [Test]
    public void WeeklyHoursMustBeLeeThan48()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WeeklyHours = 49
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.WeeklyHours);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WeeklyHours)}");
        result.Errors[0].ErrorCode.Should().Be("43");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.WeeklyHours);
    }
}