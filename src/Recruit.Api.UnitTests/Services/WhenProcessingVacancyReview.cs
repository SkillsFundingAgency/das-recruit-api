using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Models;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;
using SFA.DAS.Recruit.Api.Validators.Rules;

namespace SFA.DAS.Recruit.Api.UnitTests.Services;

public class WhenProcessingVacancyReview
{
    private class MockRule(RuleOutcome ruleOutcome) : IRule<VacancySnapshot>
    {
        public async Task<RuleOutcome> EvaluateAsync(VacancySnapshot subject, CancellationToken cancellationToken = default)
        {
            return ruleOutcome;
        }
    }

    private static readonly RuleOutcome FailedOutcome = new (RuleId.VacancyAnonymous, 100, "Narrative", "SomeField", [new RuleOutcome(RuleId.VacancyAnonymous, 100, "")]);
    private static readonly RuleOutcome PassedOutcome = new (RuleId.VacancyAnonymous, 0, "Narrative", "SomeField", [new RuleOutcome(RuleId.VacancyAnonymous, 0, "")]);
    
    [Test, MoqAutoData]
    public async Task Then_Upon_Failure_The_Automated_Qa_Outcome_Fields_Are_Set_Correctly(
        VacancyReviewEntity entity,
        VacancySnapshot snapshot)
    {
        // arrange
        entity.Status = ReviewStatus.New;
        entity.VacancySnapshot = JsonSerializer.Serialize(snapshot);
        var sut = new AutomatedReviewService([new MockRule(FailedOutcome)]);

        // act
        await sut.ProcessVacancyReviewAsync(entity, CancellationToken.None);

        // assert
        entity.Status.Should().Be(ReviewStatus.New);
        entity.AutomatedQaOutcome.Should().Be(nameof(RuleSetDecision.Refer));
        entity.AutomatedQaOutcomeIndicators.Should().Be("True");
    }
    
    [Test, MoqAutoData]
    public async Task Then_Upon_Success_The_Automated_Qa_Outcome_Fields_Are_Set_Correctly(
        VacancyReviewEntity entity,
        VacancySnapshot snapshot)
    {
        // arrange
        entity.Status = ReviewStatus.New;
        entity.VacancySnapshot = JsonSerializer.Serialize(snapshot);
        var sut = new AutomatedReviewService([new MockRule(PassedOutcome)]);

        // act
        await sut.ProcessVacancyReviewAsync(entity, CancellationToken.None);

        // assert
        entity.Status.Should().Be(ReviewStatus.New);
        entity.AutomatedQaOutcome.Should().Be(nameof(RuleSetDecision.Approve));
        entity.AutomatedQaOutcomeIndicators.Should().Be("False");
    }
}