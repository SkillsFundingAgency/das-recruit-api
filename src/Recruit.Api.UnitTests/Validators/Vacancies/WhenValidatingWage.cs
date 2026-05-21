using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingWage : VacancyValidationTestsBase
{
    [TestCase(WageType.FixedWage, 30000, null)]
    [TestCase(WageType.NationalMinimumWage, null, null)]
    [TestCase(WageType.NationalMinimumWageForApprentices, null, null)]
    public void NoErrorsWhenWageFieldsAreValid(WageType wageTypeValue, int? yearlyFixedWageAmountValue,
        string? wageAdditionalInfoValue)
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = wageTypeValue,
                FixedWageYearlyAmount = Convert.ToDecimal(yearlyFixedWageAmountValue),
                WageAdditionalInformation = wageAdditionalInfoValue
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [Test]
    public void WageMustNotBeNull()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = null,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}");
        result.Errors[0].ErrorCode.Should().Be("46");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
    }

    [Test]
    public void WageTypeMustHaveAValue()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = null
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WageType)}");
        result.Errors[0].ErrorCode.Should().Be("46");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
    }

    [Test]
    public void WageTypeMustHaveAValidValue()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = (WageType)1000
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WageType)}");
        result.Errors[0].ErrorCode.Should().Be("46");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
    }

    [TestCase(null)]
    [TestCase("")]
    public void WageAdditionalInfoIsOptional(string? descriptionValue)
    {
        var vacancy = new VacancyRequest {
            Wage = new Wage {
                WageType = WageType.NationalMinimumWage,
                WageAdditionalInformation = descriptionValue
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void WageAdditionalInfoMustContainValidCharacters(string invalidCharacter, bool expectedResult)
    {
        var vacancy = new VacancyRequest {
            Wage = new Wage {
                WageType = WageType.NationalMinimumWage,
                WageAdditionalInformation = new String('a', 50) + invalidCharacter + new String('a', 50)
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);
        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should()
                .Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WageAdditionalInformation)}");
            result.Errors[0].ErrorCode.Should().Be("45");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
        }
    }

    [Test]
    public void WageAdditionalInfoMustBeLessThan251Characters()
    {
        var vacancy = new VacancyRequest {
            Wage = new Wage {
                WageType = WageType.NationalMinimumWage,
                WageAdditionalInformation = new string('a', 252)
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should()
            .Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.WageAdditionalInformation)}");
        result.Errors[0].ErrorCode.Should().Be("44");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
    }

    [TestCase(null)]
    [TestCase("")]
    public void WageCompanyBenefitsInfoIsOptional(string? descriptionValue)
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = WageType.NationalMinimumWage,
                CompanyBenefitsInformation = descriptionValue
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    [TestCase("<p><br></p><ul><li>item1</li><li>item2</li></ul>", true)]
    [TestCase("<script>alert('not allowed')</script>", false)]
    [TestCase("<p>`</p>", false)]
    public void WageCompanyBenefitsInfoMustContainValidCharacters(string invalidCharacter, bool expectedResult)
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = WageType.NationalMinimumWage,
                CompanyBenefitsInformation = new String('a', 50) + invalidCharacter + new String('a', 50)
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        if (expectedResult)
        {
            result.HasErrors.Should().BeFalse();
        }
        else
        {
            result.HasErrors.Should().BeTrue();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should()
                .Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.CompanyBenefitsInformation)}");
            result.Errors[0].ErrorCode.Should().Be("45");
            result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
        }
    }

    [Test]
    public void WageCompanyBenefitsInfoMustBeLessThan251Characters()
    {
        var vacancy = new VacancyRequest 
        {
            Wage = new Wage 
            {
                WageType = WageType.NationalMinimumWage,
                CompanyBenefitsInformation = new string('a', 252)
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Wage);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should()
            .Be($"{nameof(vacancy.Wage)}.{nameof(vacancy.Wage.CompanyBenefitsInformation)}");
        result.Errors[0].ErrorCode.Should().Be("44");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Wage);
    }

}