using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingNumberOfPositions : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenPositionFieldsAreValid()
    {
        var vacancy = new VacancyRequest 
        {
            NumberOfPositions = 2,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.NumberOfPositions);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase(null)]
    [TestCase(0)]
    public void NumberOfPositionMustHaveAValue(int? numOfPositionsValue)
    {
        var vacancy = new VacancyRequest 
        {
            Status = VacancyStatus.Draft,
            NumberOfPositions = numOfPositionsValue,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.NumberOfPositions);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.NumberOfPositions));
        result.Errors[0].ErrorCode.Should().Be("10");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.NumberOfPositions);
    }
}