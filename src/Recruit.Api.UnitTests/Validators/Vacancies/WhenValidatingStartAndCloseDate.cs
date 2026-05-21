using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingStartAndCloseDate: VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenFieldsAreValid()
    {
        var vacancy = new VacancyRequest
        {
            StartDate = DateTime.UtcNow.AddDays(5),
            ClosingDate = DateTime.UtcNow.AddDays(4),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDateEndDate);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void StartDateCantBeTheSameAsClosingDate()
    {
        var vacancy = new VacancyRequest
        {
            StartDate = DateTime.UtcNow,
            ClosingDate = DateTime.UtcNow,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };
            
        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDateEndDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(string.Empty);
        result.Errors[0].ErrorCode.Should().Be("24");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.StartDateEndDate);
    }

    [Test]
    public void StartDateCantBeBeforeClosingDate()
    {
        var vacancy = new VacancyRequest
        {
            StartDate = DateTime.UtcNow,
            ClosingDate = DateTime.UtcNow.AddDays(1),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.StartDateEndDate);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(string.Empty);
        result.Errors[0].ErrorCode.Should().Be("24");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.StartDateEndDate);
    }
}