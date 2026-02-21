using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingAdditionalTrainingDescription: VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenDescriptionFieldIsValid()
    {
        var vacancy = new VacancyRequest 
        {
            AdditionalTrainingDescription = "a valid description",
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription | VacancyRuleSet.AdditionalTrainingDescription);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }        
    
    [Test]
    public void DescriptionMustNotBeLongerThanMaxLength()
    {
        var vacancy = new VacancyRequest 
        {
            AdditionalTrainingDescription = new String('a', 4001),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription | VacancyRuleSet.AdditionalTrainingDescription);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalTrainingDescription));
        result.Errors[0].ErrorCode.Should().Be("341");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.AdditionalTrainingDescription);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void DescriptionMustContainValidHtml(string actual, bool expectedResult)
    {
        var vacancy = new VacancyRequest 
        {
            AdditionalTrainingDescription = actual,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription | VacancyRuleSet.AdditionalTrainingDescription);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalTrainingDescription));
            result.Errors[0].ErrorCode.Should().Be("344");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.AdditionalTrainingDescription);
        }
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void Description_Should_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new VacancyRequest
        {
            AdditionalTrainingDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingDescription | VacancyRuleSet.AdditionalTrainingDescription);
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.AdditionalTrainingDescription));
        result.Errors[0].ErrorCode.Should().Be("342");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.AdditionalTrainingDescription);
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void Description_Should_Not_Fail_IfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new VacancyRequest
        {
            AdditionalTrainingDescription = freeText,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.AdditionalTrainingDescription);
        result.HasErrors.Should().BeFalse();
    }
}