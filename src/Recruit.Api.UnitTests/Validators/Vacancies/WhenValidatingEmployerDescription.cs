using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingEmployerDescription : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenEmployerDescriptionFieldIsValid()
    {
        var vacancy = new PutVacancyRequest 
        {
            EmployerDescription = "a valid description",
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase(null)]
    [TestCase("")]
    public void DescriptionMustNotBeEmpty(string? description)
    {
        var vacancy = new PutVacancyRequest 
        {
            EmployerDescription = description,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerDescription));
        result.Errors[0].ErrorCode.Should().Be("80");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerDescription);
    }

    [Test]
    public void EmployerDescriptionMustNotBeLongerThanMaxLength()
    {
        var vacancy = new PutVacancyRequest 
        {
            EmployerDescription = new string('a', 4001),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerDescription));
        result.Errors[0].ErrorCode.Should().Be("77");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerDescription);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void EmployerDescriptionMustContainValidCharacters(string invalidChar, bool expectedResult)
    {
        var vacancy = new PutVacancyRequest 
        {
            EmployerDescription = invalidChar,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerDescription);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerDescription));
            result.Errors[0].ErrorCode.Should().Be("78");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerDescription);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void EmployerDescription_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            Description = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
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
    public void EmployerDescription_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            Description = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Description);
        result.HasErrors.Should().BeFalse();
    }
}