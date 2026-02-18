using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.VacancyEntity;

namespace SFA.DAS.Recruit.Api.UnitTests.Validators.Vacancies;

public class WhenValidatingTrainingProvider: VacancyValidationTestsBase
{
    [Test]
    public void NoErrorsWhenTrainingProviderUkprnIsValid()
    {
        var vacancy = new Vacancy
        {
            TrainingProvider = new TrainingProvider { Ukprn = 12345678 },
            Status = VacancyStatus.Draft
        };

        //TODO FAI-2972
        //MockTrainingProviderSummaryProvider.Setup(p => p.GetAsync(12345678)).ReturnsAsync(new TrainingProviderSummary{IsTrainingProviderMainOrEmployerProfile = true});
        
        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProvider);

        result.HasErrors.Should().BeFalse();
        result.Errors.Should().HaveCount(0);
    }

    //TODO FAI-2972
    // [Test]
    // public void ErrorIfTrainingProviderIsBlocked()
    // {
    //     var vacancy = new Vacancy
    //     {
    //         TrainingProvider = new TrainingProvider { Ukprn = 12345678 },
    //         Status = VacancyStatus.Draft
    //     };
    //
    //     MockTrainingProviderSummaryProvider.Setup(p => p.GetAsync(12345678)).ReturnsAsync(new TrainingProviderSummary{IsTrainingProviderMainOrEmployerProfile = true});
    //
    //     MockBlockedOrganisationRepo.Setup(b => b.GetByOrganisationIdAsync("12345678"))
    //         .ReturnsAsync(new BlockedOrganisation {BlockedStatus = BlockedStatus.Blocked});
    //
    //     var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProvider);
    //
    //     result.HasErrors.Should().BeTrue();
    //     result.Errors.Should().HaveCount(1);
    //     result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingProvider));
    //     result.Errors[0].ErrorCode.Should().Be(ErrorCodes.TrainingProviderMustNotBeBlocked);
    //     result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProvider);
    // }

    [Test]
    public void ErrorIfTrainingProviderIsNull()
    {
        var vacancy = new Vacancy
        {
            TrainingProvider = null,
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProvider);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingProvider));
        result.Errors[0].ErrorCode.Should().Be("101");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProvider);
    }

    [Test]
    public void EmptyUkprnNotAllowed()
    {
        var vacancy = new Vacancy
        {
            TrainingProvider = new TrainingProvider { Ukprn = null },
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProvider);

        result.HasErrors.Should().BeTrue();
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingProvider));
        result.Errors[0].ErrorCode.Should().Be("101");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProvider);
    }

    [TestCase(0L)]
    [TestCase(1234L)]
    [TestCase(123456789L)]
    public void TrainingProviderUkprnMustBe8Digits(long? ukprn)
    {
        var vacancy = new Vacancy
        {
            TrainingProvider = new TrainingProvider { Ukprn = ukprn },
            Status = VacancyStatus.Draft
        };

        var result = Validator.Validate(vacancy, VacancyRuleSet.TrainingProvider);

        result.HasErrors.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be(nameof(vacancy.TrainingProvider));
        result.Errors[0].ErrorCode.Should().Be("99");
        result.Errors[0].RuleId.Should().Be((long)VacancyRuleSet.TrainingProvider);
    }
    
}