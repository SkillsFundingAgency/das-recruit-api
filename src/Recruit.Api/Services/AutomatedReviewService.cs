using System.Text.Json;
using SFA.DAS.Recruit.Api.Domain.Configuration;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Validators.Rules;

namespace SFA.DAS.Recruit.Api.Services;

public interface IAutomatedReviewService
{
    Task ProcessVacancyReviewAsync(VacancyReviewEntity entity, CancellationToken cancellationToken = default);
}

public class AutomatedReviewService(IEnumerable<IRule<VacancySnapshot>> vacancyRules): IAutomatedReviewService
{
    public async Task ProcessVacancyReviewAsync(VacancyReviewEntity entity, CancellationToken cancellationToken = default)
    {
        var snapshot = JsonSerializer.Deserialize<VacancySnapshot>(entity.VacancySnapshot, JsonConfig.Options)!;
        var outcome = await vacancyRules.EvaluateAsync(snapshot, cancellationToken);
        entity.AutomatedQaOutcome = outcome.Decision.ToString();
        entity.AutomatedQaOutcomeIndicators = JsonSerializer.Serialize(outcome
            .RuleOutcomes
            .Where(x => x is { Details: not null, Score: > 0})
            .SelectMany(o => o.Details!)
            .Where(s => s.Score > 0)
            .ToList());
    }
}