using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingStartDate : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenStartDateIsValid()
    {
        var vacancy = new PutVacancyRequest
        {
            ClosingDate = DateTime.Today.AddDays(14), 
            StartDate = DateTime.UtcNow.AddDays(15),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDate);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void StartDateMustHaveAValue()
    {
        var vacancy = new PutVacancyRequest 
        {
            StartDate = null,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };
        
        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.StartDate)}");
        result.Errors[0].ErrorCode.Should().Be("20");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.StartDate);
    }

    [Test]
    [TestCaseSource(nameof(InvalidDaysFromClosingDate))]
    public void StartDateMustBeGreaterThanToday(int startDate)
    {
        var closingDate = DateTime.Today.AddDays(50);
        var vacancy = new PutVacancyRequest
        {
            ClosingDate = closingDate,
            StartDate = closingDate.AddDays(startDate),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.StartDate)}");
        result.Errors[0].ErrorCode.Should().Be("22");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.StartDate);
    }
    
    private static IEnumerable<object[]> InvalidDaysFromClosingDate =>
        new List<object[]>
        {
            new object[] { -1 },
            new object[] { -2 },
            new object[] { -15 }
        };
}