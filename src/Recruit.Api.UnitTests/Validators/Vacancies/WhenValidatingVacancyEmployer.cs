using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingVacancyEmployer : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenEmployerFieldsAreValid()
    {
        var vacancy = new VacancyRequest
        {
            EmployerName = "Test Org",
            EmployerLocations = [
                new Address
                {
                    AddressLine1 = "1 New Street",
                    Postcode = "AB1 3SD"
                }
            ],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
            
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerName | VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    
    [TestCase(null)]
    [TestCase("")]
    public void EmployerMustBeSet(string? organisationValue)
    {
        var vacancy = new VacancyRequest 
        {
            EmployerName = organisationValue,
            SourceOrigin = SourceOrigin.ProviderWeb,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerName);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerName));
        result.Errors[0].ErrorCode.Should().Be("4");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerName);
    }

    [Test]
    public void ShouldErrorIfEmployerLocationIsNull()
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = null,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.EmployerLocations));
        result.Errors[0].ErrorCode.Should().Be("98");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmployerAddressLine1MustBeSet(string? addressValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = addressValue,
                Postcode = "AB12 3DZ"   
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine1)}");
        result.Errors[0].ErrorCode.Should().Be("5");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase("<")]
    [TestCase(">")]
    public void EmployerAddressLine1MustContainValidCharacters(string testValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = testValue,
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine1)}");
        result.Errors[0].ErrorCode.Should().Be("6");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [Test]
    public void EmployerAddressLine1CannotBeLongerThan100Characters()
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = new string('a', 101),
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine1)}");
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase("<")]
    [TestCase(">")]
    public void EmployerAddressLine2MustContainValidCharacters(string testValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine2 = testValue,
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine2)}");
        result.Errors[0].ErrorCode.Should().Be("6");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [Test]
    public void EmployerAddressLine2CannotBeLongerThan100Characters()
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine2 = new String('a', 101),
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine2)}");
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase("<")]
    [TestCase(">")]
    public void EmployerAddressLine3MustContainValidCharacters(string testValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine3 = testValue,
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine3)}");
        result.Errors[0].ErrorCode.Should().Be("6");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase]
    public void EmployerAddressLine3CannotBeLongerThan100Characters()
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine3 = new string('a', 101),
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine3)}");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
        result.Errors[0].ErrorCode.Should().Be("7");
    }

    [TestCase("<")]
    [TestCase(">")]
    public void EmployerAddressLine4MustContainValidCharacters(string testValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine4 = testValue,
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine4)}");
        result.Errors[0].ErrorCode.Should().Be("6");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [Test]
    public void EmployerAddressLine4CannotBeLongerThan100Characters()
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                AddressLine4 = new string('a', 101),
                Postcode = "AB12 3DZ"
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.AddressLine4)}");
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmployerPostCodeMustBeSet(string? postCodeValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = [new Address
            {
                AddressLine1 = "2 New Street",
                Postcode = postCodeValue
            }],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.Postcode)}");
        result.Errors[0].ErrorCode.Should().Be("8");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }

    [TestCase("12345")]
    [TestCase("AAAAAA")]
    [TestCase("AS123 1JJ")]
    public void EmployerPostCodeMustBeValidFormat(string postCodeValue)
    {
        var vacancy = new VacancyRequest
        {
            EmployerLocations = 
            [
                new Address
                {
                    AddressLine1 = "2 New Street",
                    Postcode = postCodeValue
                }
            ],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.EmployerAddress);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.EmployerLocations)}[0].{nameof(Address.Postcode)}");
        result.Errors[0].ErrorCode.Should().Be("9");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.EmployerAddress);
    }
}