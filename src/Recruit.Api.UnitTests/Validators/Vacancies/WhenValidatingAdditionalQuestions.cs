using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingAdditionalQuestions: VacancyValidationTestsBase
{
    [TestCase("a valid AdditionalQuestion1?")]
    [TestCase("a valid? AdditionalQuestion1")]
    [TestCase("")]
    public void NoErrorsWhenAdditionalQuestion1FieldIsValid(string text)
    {
        var vacancy = new Vacancy 
        {
            AdditionalQuestion1 = text,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);
        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    [Test]
    public void AdditionalQuestion1MustNotBeLongerThanMaxLength()
    {
        var vacancy = new Vacancy 
        {
            AdditionalQuestion1 = new string('?', 251 ),
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion1);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion1));
        result.Errors[0].ErrorCode.Should().Be("321");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.AdditionalQuestion1);
    }
    
    [TestCase("some text bother?")]
    [TestCase("some text dang?")]
    [TestCase("some text drat?")]
    [TestCase("some text balderdash?")]
    public void AdditionalQuestion1_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy()
        {
            AdditionalQuestion1 = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion1);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion1));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("322");
    }

    [TestCase("some textbother?")]
    [TestCase("some textdang?")]
    [TestCase("some textdrat?")]
    [TestCase("some textbalderdash?")]
    public void AdditionalQuestion1_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy()
        {
            AdditionalQuestion1 = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion1);
        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public void AdditionalQuestion1_MustContainQuestionMark()
    {
        var vacancy = new Vacancy
        {
            AdditionalQuestion1 = "some text?",
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion1);
        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public void AdditionalQuestion1_ShouldHaveErrorsIfDoesNotHaveQuestionMark()
    {
        var vacancy = new Vacancy
        {
            AdditionalQuestion1 = "some text",
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion1);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion1));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("340");
    }
    
    [TestCase("a valid AdditionalQuestion1?")]
    [TestCase("a valid? AdditionalQuestion1?")]
    [TestCase("")]
    public void NoErrorsWhenAdditionalQuestion2FieldIsValid(string text)
    {
        var vacancy = new Vacancy 
        {
            AdditionalQuestion2 = text,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    [Test]
    public void AdditionalQuestion2MustNotBeLongerThanMaxLength()
    {
        var vacancy = new Vacancy 
        {
            AdditionalQuestion2 = new string('?', 251),
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion2));
        result.Errors[0].ErrorCode.Should().Be("321");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.AdditionalQuestion2);
    }
    
    [TestCase("some text bother?")]
    [TestCase("some text dang?")]
    [TestCase("some text? drat")]
    [TestCase("some text? balderdash")]
    public void AdditionalQuestion2ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy()
        {
            AdditionalQuestion2 = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion2));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("322");
    }

    [TestCase("some textbother?")]
    [TestCase("some textdang?")]
    [TestCase("some textdrat?")]
    [TestCase("some textbalderdash?")]
    public void AdditionalQuestion2_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new Vacancy
        {
            AdditionalQuestion2 = freeText,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);
        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public void AdditionalQuestion2_MustContainQuestionMark()
    {
        var vacancy = new Vacancy
        {
            AdditionalQuestion2 = "some text?",
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);
        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public void AdditionalQuestion2_ShouldHaveErrorsIfDoesNotHaveQuestionMark()
    {
        var vacancy = new Vacancy
        {
            AdditionalQuestion2 = "some text",
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalQuestion2);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalQuestion2));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("340");
    }
}