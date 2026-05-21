namespace SFA.DAS.Recruit.Api.Validators.Rules;

public interface IRule<in TSubject>
{
    Task<RuleOutcome> EvaluateAsync(TSubject subject, CancellationToken cancellationToken = default);
}