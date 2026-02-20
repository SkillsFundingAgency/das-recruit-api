using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingOutcomeDescription : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenOutcomeDescriptionFieldIsValid()
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = "a valid description",
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase(null)]
    [TestCase("")]
    public void OutcomeDescriptionMustNotBeEmpty(string? outcomeDescription)
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = outcomeDescription,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.OutcomeDescription));
        result.Errors[0].ErrorCode.Should().Be("55");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.OutcomeDescription);
    }

    [Test]
    public void OutcomeDescriptionMustNotBeLongerThanMaxLength()
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = new string('a', 4001),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.OutcomeDescription));
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.OutcomeDescription);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void OutcomeDescriptionMustContainValidHtml(string actual, bool expectedResult)
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = actual,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.OutcomeDescription));
            result.Errors[0].ErrorCode.Should().Be("6");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.OutcomeDescription);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void OutcomeDescription_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.OutcomeDescription));
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("611");
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void OutcomeDescription_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new VacancyRequest 
        {
            OutcomeDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.OutcomeDescription);
        result.HasErrors.Should().BeFalse();
    }
}