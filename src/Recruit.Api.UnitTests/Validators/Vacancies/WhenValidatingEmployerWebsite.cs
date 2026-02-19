using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingEmployerWebsite: VacancyValidationTestsBase
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("http://www.company.com")]
    [TestCase("https://www.company.com")]
    public void NoErrorsWhenEmployerWebsiteUrlIsValid(string? url)
    {
        var vacancy = new PutVacancyRequest
        {
            EmployerWebsiteUrl = url,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerWebsiteUrl);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
        
    [Test]
    public void EmployerWebsiteUrlMustBe100CharactersOrLess()
    {
        var vacancy = new PutVacancyRequest
        {
            EmployerWebsiteUrl = "http://www.company.com".PadRight(101, 'w'),
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerWebsiteUrl);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerWebsiteUrl));
        result.Errors[0].ErrorCode.Should().Be("84");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerWebsiteUrl);
    }

    [TestCase("invalid url")]
    [TestCase("company")]
    [TestCase("domain.com ?term=query")]
    [TestCase(".com")]
    [TestCase(".org.uk")]
    [TestCase(",com")]
    [TestCase("company.com")]
    [TestCase("www.company.com")]
    [TestCase("/apply")]
    [TestCase("/apply?source=foo")]
    [TestCase("/apply.aspx")]
    public void EmployerWebsiteUrlMustBeAValidWebAddress(string invalidUrl)
    {
        var vacancy = new PutVacancyRequest
        {
            EmployerWebsiteUrl = invalidUrl,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerWebsiteUrl);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerWebsiteUrl));
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerWebsiteUrl);
    }
}