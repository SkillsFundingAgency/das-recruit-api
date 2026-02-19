using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingExternalApplicationInstructions : VacancyValidationTestsBase
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("You can apply online or through post.")]
    public void NoErrorsWhenExternalApplicationInstructionsIsValid(string? instructions)
    {
        var vacancy = new PutVacancyRequest 
        {
            ApplicationMethod = ApplicationMethod.ThroughExternalApplicationSite,
            ApplicationUrl = "http://www.apply.com",
            ApplicationInstructions = instructions,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ApplicationMethod);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void ExternalApplicationInstructionsMustBe500CharactersOrLess()
    {
        var vacancy = new PutVacancyRequest 
        {
            ApplicationMethod = ApplicationMethod.ThroughExternalApplicationSite,
            ApplicationUrl = "http://www.apply.com",
            ApplicationInstructions = "instructions".PadRight(501, 'w'),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ApplicationMethod);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ApplicationInstructions));
        result.Errors[0].ErrorCode.Should().Be("88");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ApplicationMethod);
    }

    [TestCase("<script>alert('not allowed')</script>")]
    [TestCase("<p>`</p>")]
    public void ExternalApplicationInstructionsMustNotContainInvalidCharacters(string invalidChar)
    {
        var vacancy = new PutVacancyRequest 
        {
            ApplicationMethod = ApplicationMethod.ThroughExternalApplicationSite,
            ApplicationUrl = "http://www.apply.com",
            ApplicationInstructions = invalidChar,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.ApplicationMethod);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.ApplicationInstructions));
        result.Errors[0].ErrorCode.Should().Be("89");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.ApplicationMethod);
    }
}
