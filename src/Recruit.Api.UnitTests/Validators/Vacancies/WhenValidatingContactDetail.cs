using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingContactDetail : VacancyValidationTestsBase
{
    [TestCase(null,VacancyRuleSet.EmployerContactDetails)]
    [TestCase("",VacancyRuleSet.EmployerContactDetails)]
    [TestCase("joebloggs@company.com",VacancyRuleSet.EmployerContactDetails)]
    [TestCase(null,VacancyRuleSet.ProviderContactDetails)]
    [TestCase("",VacancyRuleSet.ProviderContactDetails)]
    [TestCase("joebloggs@company.com",VacancyRuleSet.ProviderContactDetails)]
    public void NoErrorsWhenEmployerContactEmailIsValid(string? emailAddress, VacancyRuleSet ruleSet)
    {
        var vacancy = new PutVacancyRequest {
            Contact = new ContactDetail { Email = emailAddress },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, ruleSet);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void EmployerContactEmailMustBe100CharactersOrLess()
    {
        var vacancy = new PutVacancyRequest {
            Contact = new ContactDetail { Email = "name@".PadRight(101, 'w') },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerContactDetails);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should()
            .Be($"{nameof(vacancy.Contact)}.{nameof(vacancy.Contact.Email)}");
        result.Errors[0].ErrorCode.Should().Be("92");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerContactDetails | (long)VacancyRuleSet.ProviderContactDetails);
    }

    [TestCase("<script>alert('not allowed')</script>")]
    [TestCase("<p>`</p>")]
    public void EmployerContactEmailMustNotContainInvalidCharacters(string invalidChar)
    {
        var vacancy = new PutVacancyRequest {
            Contact = new ContactDetail { Email = invalidChar },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerContactDetails);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should()
            .Be($"{nameof(vacancy.Contact)}.{nameof(vacancy.Contact.Email)}");
        result.Errors[0].ErrorCode.Should().Be("93");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerContactDetails | (long)VacancyRuleSet.ProviderContactDetails);
    }

    [Test]
    public void EmployerContactEmailMustBeValidEmailFormat()
    {
        var vacancy = new PutVacancyRequest {
            Contact = new ContactDetail { Email = "joe" },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerContactDetails);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should()
            .Be($"{nameof(vacancy.Contact)}.{nameof(vacancy.Contact.Email)}");
        result.Errors[0].ErrorCode.Should().Be("94");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerContactDetails | (long)VacancyRuleSet.ProviderContactDetails);
    }
}