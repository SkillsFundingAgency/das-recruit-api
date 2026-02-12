using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingDescription : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenDescriptionFieldIsValid()
    {
        var vacancy = new Vacancy {
            Description = "a valid description",
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase(null, "will do at work")]
    [TestCase("", "will do at work")]
    public void DescriptionMustNotBeEmpty(string? description, string errorMessage)
    {
        var vacancy = new Vacancy {
            Description = description,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.Description));
        result.Errors[0].ErrorCode.Should().Be("53");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Description);
        result.Errors[0].ErrorMessage.Should().Contain(errorMessage);
    }

    [Test]
    public void DescriptionMustNotBeLongerThanMaxLength()
    {
        var vacancy = new Vacancy {
            Description = new String('a', 4001),
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.Description));
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Description);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void DescriptionMustContainValidHtml(string actual, bool expectedResult)
    {
        var vacancy = new Vacancy {
            Description = actual,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.Description));
            result.Errors[0].ErrorCode.Should().Be("6");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Description);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void Description_Should_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy {
            Description = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.Description));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("609");
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void Description_Should_Not_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy {
            Description = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);
        result.HasErrors.Should().BeFalse();
    }
}