namespace SFA.DAS.Recruit.Api.Validators.Rules;

public class RuleSetOutcome
{
    public List<RuleOutcome> RuleOutcomes { get; set; } = [];
    public RuleSetDecision Decision { get; set; }
}