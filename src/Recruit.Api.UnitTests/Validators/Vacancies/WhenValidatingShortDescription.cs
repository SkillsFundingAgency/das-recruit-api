using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingShortDescription : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenShortDescriptionFieldsAreValid()
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = new string('a', 60)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    [TestCase(null)]
    [TestCase("")]
    public void ShortDescriptionMustHaveAValue(string? shortDescriptionValue)
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = shortDescriptionValue
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ShortDescription));
        result.Errors[0].ErrorCode.Should().Be("12");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ShortDescription);
    }

    [Test]
    public void NoErrorsWhenShortDescriptionAreValid()
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = new string('a', 350)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void ShortDescriptionMustNotBeMoreThan350Characters()
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = new string('a', 351)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ShortDescription));
        result.Errors[0].ErrorCode.Should().Be("13");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ShortDescription);
    }

    [Test]
    public void ShortDescriptionMustNotBeLessThan50Characters()
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = new string('a', 49)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ShortDescription));
        result.Errors[0].ErrorCode.Should().Be("14");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ShortDescription);
    }


    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void ShortDescriptionMustContainValidCharacters(string invalidCharacter, bool expectedResult)
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = new String('a', 30) + invalidCharacter + new String('b', 30)
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ShortDescription));
            result.Errors[0].ErrorCode.Should().Be("15");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ShortDescription);
        }
    }

    [TestCase("some text bother random text random text random text random text")]
    [TestCase("some text dang random text random text random text random text")]
    [TestCase("some text drat random text random text random text random text")]
    [TestCase("some text balderdash random text random text random text random text")]
    public void ShortDescription_Should_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy()
        {
            Status = VacancyStatus.Draft,
            ShortDescription = freeText
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ShortDescription));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("605");
    }

    [TestCase("some textbother random text random text random text random text")]
    [TestCase("some textdang random text random text random text random text")]
    [TestCase("some textdrat random text random text random text random text")]
    [TestCase("some textbalderdash random text random text random text random text")]
    public void ShortDescription_Should_Not_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy
        {
            Status = VacancyStatus.Draft,
            ShortDescription = freeText
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ShortDescription);
        result.HasErrors.Should().BeFalse();
    }
}