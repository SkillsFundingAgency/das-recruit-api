using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingProgrammeId : VacancyValidationTestsBase
{
    [Test]
    public void ErrorWhenProgrammeIsNull()
    {
        var vacancy = new PutVacancyRequest
        {
            ProgrammeId = null,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProgramme);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.ProgrammeId)}");
        result.Errors[0].ErrorCode.Should().Be("25");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProgramme);
    }

    [Test]
    public void NoErrorsWhenProgrammeIdIsValid()
    {
        var vacancy = new PutVacancyRequest
        {
            ProgrammeId = "123",
            TrainingProvider = new TrainingProvider
            {
                Ukprn = 10000000
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProgramme);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    
    [TestCase("")]
    [TestCase("     ")]
    [TestCase(null)]
    public void IdMustHaveAValue(string? idValue)
    {
        var vacancy = new PutVacancyRequest
        {
            ProgrammeId = idValue,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProgramme);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.ProgrammeId)}");
        result.Errors[0].ErrorCode.Should().Be("25");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProgramme);
    }
}