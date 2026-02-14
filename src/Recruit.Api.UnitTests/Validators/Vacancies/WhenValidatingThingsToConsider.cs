using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingThingsToConsider : VacancyValidationTestsBase
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("considerations")]
    public void NoErrorsWhenThingsToConsiderIsValid(string? text)
    {
        var vacancy = new Vacancy 
        {
            ThingsToConsider = text,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ThingsToConsider);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void ThingsToConsiderMustBe4000CharactersOrLess()
    {
        var vacancy = new Vacancy 
        {
            ThingsToConsider = "name".PadRight(4001, 'w'),
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ThingsToConsider);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ThingsToConsider));
        result.Errors[0].ErrorCode.Should().Be("75");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ThingsToConsider);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void ThingsToConsiderMustNotContainInvalidCharacters(string invalidChar, bool expectedResult)
    {
        var vacancy = new Vacancy 
        {
            ThingsToConsider = invalidChar,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ThingsToConsider);
        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ThingsToConsider));
            result.Errors[0].ErrorCode.Should().Be("76");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ThingsToConsider);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void ThingsToConsider_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy 
        {
            ThingsToConsider = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ThingsToConsider);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ThingsToConsider));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("613");
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void ThingsToConsider_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy 
        {
            ThingsToConsider = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ThingsToConsider);
        result.HasErrors.Should().BeFalse();
    }

}