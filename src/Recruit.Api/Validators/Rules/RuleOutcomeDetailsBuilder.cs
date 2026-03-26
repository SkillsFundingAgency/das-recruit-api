using System.Text;

namespace SFA.DAS.Recruit.Api.Validators.Rules;

public class RuleOutcomeDetailsBuilder(RuleId ruleId, string target = RuleOutcome.NoSpecificTarget)
{
    private int _totalScore;
    private int _outcomeCount;
    private readonly StringBuilder _narrative = new();
    private readonly List<RuleOutcome> _details = [];

    public RuleOutcomeDetailsBuilder Add(IEnumerable<RuleOutcome> outcomes)
    {
        foreach (var outcome in outcomes)
        {
            if (outcome.RuleId != ruleId)
            {
                throw new ArgumentException($"Invalid rule ID specified '{outcome.RuleId}' (does not match existing rule ID '{ruleId}' in the outcome)", nameof(outcome.RuleId));
            }

            _totalScore += outcome.Score;
            _outcomeCount++;
            _narrative.AppendLine(outcome.Narrative);
            _details.Add(outcome);
        }

        return this;
    }

    public RuleOutcome ComputeSum(string? narrative = null)
    {
        EnsureThereAreOutcomes();
        return new RuleOutcome(ruleId, _totalScore, narrative ?? _narrative.ToString(), target, _details);
    }

    private void EnsureThereAreOutcomes()
    {
        if (_outcomeCount == 0)
        {
            throw new InvalidOperationException("Cannot compute score from outcome details because there are no details");
        }
    }
}