using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingTrainingDescription : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenTrainingDescriptionFieldIsValid()
    {
        var vacancy = new PutVacancyRequest 
        {
            TrainingDescription = "a valid description",
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void TrainingDescriptionMustNotBeLongerThanMaxLength()
    {
        var vacancy = new PutVacancyRequest
        {
            TrainingDescription = new string('a', 4001),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingDescription));
        result.Errors[0].ErrorCode.Should().Be("321");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingDescription);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void TrainingDescriptionMustContainValidHtml(string actual, bool expectedResult)
    {
        var vacancy = new PutVacancyRequest 
        {
            TrainingDescription = actual,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingDescription));
            result.Errors[0].ErrorCode.Should().Be("346");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingDescription);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void TrainingDescription_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            TrainingDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingDescription));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("322");
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void TrainingDescription_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            TrainingDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription);
        result.HasErrors.Should().BeFalse();
    }

}