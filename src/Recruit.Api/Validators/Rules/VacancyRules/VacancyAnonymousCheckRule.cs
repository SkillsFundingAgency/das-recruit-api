using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;

namespace SFA.DAS.Recruit.Api.Validators.Rules.VacancyRules;

public sealed class VacancyAnonymousCheckRule : IRule<VacancySnapshot>
{
    public Task<RuleOutcome> EvaluateAsync(VacancySnapshot subject, CancellationToken cancellationToken = default)
    {
        var score = subject.EmployerNameOption == EmployerNameOption.Anonymous ? 100 : 0;
        var ruleOutcome = new RuleOutcome(RuleId.VacancyAnonymous, score, "Anonymous employer", nameof(Vacancy.EmployerName));

        var outcomeBuilder = new RuleOutcomeDetailsBuilder(RuleId.VacancyAnonymous);
        var outcome = outcomeBuilder
            .Add([ruleOutcome])
            .ComputeSum();

        return Task.FromResult(outcome);
    }
}