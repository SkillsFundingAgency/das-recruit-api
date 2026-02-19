using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Requests.Vacancy;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingSkills : VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenSkillsAreValid()
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = 
            [
                new string('a', 30)
            ],
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }
    
    [TestCaseSource(nameof(NullOrZeroSkillCollection))]
    public void SkillsCollectionMustNotBeNullOrHaveZeroCount(List<string> skills)
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = skills,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.Skills));
        result.Errors[0].ErrorCode.Should().Be("51");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Skills);
    }

    private static IEnumerable<object[]> NullOrZeroSkill =>
        new List<object[]> {
            new object[] { new List<string?> { null } },
            new object[] { new List<string> { string.Empty } },
        };

    [TestCaseSource(nameof(NullOrZeroSkill))]
    public void SkillMustNotBeEmpty(List<string?> skills)
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = skills,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Skills)}[0]");
        result.Errors[0].ErrorCode.Should().Be("99");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Skills);
    }

    [Test]
    public void SkillsMustNotContainInvalidCharacters()
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = new List<string> 
            {
                "<"
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Employer
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Skills)}[0]");
        result.Errors[0].ErrorCode.Should().Be("6");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Skills);
    }

    [Test]
    public void SkillsMustNotBeGreaterThanMaxLength()
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = new List<string> 
            {
                new string('a', 31)
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Skills)}[0]");
        result.Errors[0].ErrorCode.Should().Be("7");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.Skills);
    }

    [TestCase("some text bother")]
    [TestCase("some text dang")]
    [TestCase("some text drat")]
    [TestCase("some text balderdash")]
    public void OutcomeDescription_ShouldFailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = new List<string> 
            {
                freeText
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);
        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be($"{nameof(vacancy.Skills)}[0]");
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorCode.Should().Be("608");
    }

    [TestCase("some textbother")]
    [TestCase("some textdang")]
    [TestCase("some textdrat")]
    [TestCase("some textbalderdash")]
    public void OutcomeDescription_Should_Not_FailIfContainsWordsFromTheProfanityList(string freeText)
    {
        var vacancy = new PutVacancyRequest 
        {
            Skills = new List<string> 
            {
                freeText
            },
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);
        result.HasErrors.Should().BeFalse();
    }

    [Test]
    public void Skills_Are_Not_Required_For_Foundation_Apprenticeships()
    {
        var vacancy = new PutVacancyRequest 
        {
            ApprenticeshipType = ApprenticeshipTypes.Foundation,
            Skills = null,
            Status = VacancyStatus.Draft,
            OwnerType = OwnerType.Provider
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.Skills);
        result.HasErrors.Should().BeFalse();
    }
    
    private static IEnumerable<object[]> NullOrZeroSkillCollection =>
        new List<object[]> {
            new object[] { null },
            new object[] { new List<string>() },
        };
}