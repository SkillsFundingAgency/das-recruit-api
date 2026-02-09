using Microsoft.Extensions.Time.Testing;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingClosingDate : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenClosingDateIsValid()
    {
        var vacancy = new Vacancy
        {
            ClosingDate = DateTime.UtcNow.AddDays(15),
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void ClosingDateMustHaveAValue()
    {
        var vacancy = new Vacancy
        {
            ClosingDate = null,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.ClosingDate)}");
        result.Errors[0].ErrorCode.Should().Be("16");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ClosingDate);
    }

    [Test]
    [TestCaseSource(nameof(InvalidClosingDates))]
    public void ClosingDateMustBeGreaterThanToday(DateTime closingDateValue)
    {
        var vacancy = new Vacancy
        {
            ClosingDate = closingDateValue,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);

        var error = result.Errors[0];
        error.PropertyName.Should().Be(nameof(Vacancy.ClosingDate));
        error.RuleId.Should().Be((long)VacancyRuleSet.ClosingDate);
        error.ErrorCode.Should().Be("18");
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.ClosingDate)}");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ClosingDate);
    }

    [Test]
    public void NewAdvert_ClosingDateLessThan7DaysFromToday_IsInvalid()
    {
        var stubTime = new FakeTimeProvider(new DateTimeOffset(new DateTime(2025, 01, 01) ));
        TimeProvider = stubTime;

        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ClosingDate = stubTime.GetUtcNow().Date.AddDays(6)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);

        var error = result.Errors[0];
        error.PropertyName.Should().Be(nameof(Vacancy.ClosingDate));
        error.RuleId.Should().Be((long)VacancyRuleSet.ClosingDate);
        error.ErrorCode.Should().Be("18");
    }

    [Test]
    public void NewAdvert_ClosingDateExactly7DaysFromToday_IsValid()
    {
        var stubTime = new FakeTimeProvider(new DateTimeOffset(new DateTime(2025, 01, 01) ));
        TimeProvider = stubTime;

        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ClosingDate = stubTime.GetUtcNow().Date.AddDays(7)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void LiveExtension_ClosingDateCannotBeInPast_AllowsToday()
    {
        var stubTime = new FakeTimeProvider(new DateTimeOffset(new DateTime(2025, 01, 01), TimeSpan.Zero ));
        TimeProvider = stubTime;

        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Live,
            ClosingDate = stubTime.GetUtcNow().Date.AddDays(-1) // yesterday
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);

        vacancy = new Vacancy
        {
            Status = VacancyStatus.Live,
            ClosingDate = stubTime.GetUtcNow().Date // today
        };
        var resultToday = Validator.Validate(vacancy, VacancyRuleSet.ClosingDate);
        Assert.That(resultToday, Is.Not.Null);
        Assert.That(resultToday.Errors, Is.Empty);
    }
    
    private static IEnumerable<object[]> InvalidClosingDates =>
        new List<object[]>
        {
            new object[] { DateTime.UtcNow.Date },
            new object[] { DateTime.UtcNow },
        };
}