using System.Linq.Expressions;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Extensions;
using ProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.Validators.Rules.VacancyRules;

public class ProfanityData
{
    public string Profanity { get; set; }
    public int Occurrences { get; set; }
}
    
public sealed class VacancyProfanityChecksRule(IProhibitedContentRepository prohibitedContentRepository, decimal weighting = 100.0m) : IRule<VacancySnapshot>
{
    private RuleOutcome CreateOutcome(int score, string narrative, string? data, string target = RuleOutcome.NoSpecificTarget)
    {
        return new RuleOutcome(RuleId.ProfanityChecks, (int) (score * weighting), narrative, target, null, data);
    }

    private IEnumerable<string> ProfanityList { get; set; } = new List<string>();

    private IEnumerable<RuleOutcome> ProfanityCheckAsync(Expression<Func<string?>> property, string? relatedFieldId = null)
    {
        var fieldId = relatedFieldId ?? property.GetQualifiedFieldId();

        var foundProfanities = FindOccurrences(property);

        return foundProfanities.Values.Sum() > 0
            ? CreateUnconsolidatedOutcomes(foundProfanities, fieldId)
            : [CreateOutcome(0, $"No profanities found in '{fieldId}'", null, fieldId)];
    }

    private Dictionary<string, int> FindOccurrences(Expression<Func<string?>> property)
    {
        var foundProfanities = new Dictionary<string, int>();
        var value = property.Compile()();
        if (string.IsNullOrWhiteSpace(value)) return foundProfanities;
        var checkValue = value.FormatForParsing();

        foreach (var profanity in ProfanityList)
        {
            var occurrences = checkValue.CountOccurrences(profanity);
            if (occurrences > 0)
            {
                foundProfanities.TryAdd(profanity, 0);
                foundProfanities[profanity] += occurrences;
            }
        }

        return foundProfanities;
    }

    private IEnumerable<RuleOutcome> CreateUnconsolidatedOutcomes(Dictionary<string, int> foundProfanities, string fieldId)
    {
        return foundProfanities
            .Select(foundProfanity =>
            {
                var count = foundProfanity.Value;
                var term = foundProfanity.Key;
                var foundMessage = count > 1 ? $"found {count} times" : "found";
                var narrative = $"Profanity '{term}' {foundMessage} in '{fieldId}'";
                var data = JsonConvert.SerializeObject(new ProfanityData { Profanity = term, Occurrences = count });

                return CreateOutcome(count, narrative, data, fieldId);
            });
    }
        
    public async Task<RuleOutcome> EvaluateAsync(VacancySnapshot subject, CancellationToken cancellationToken = default)
    {
        var outcomeBuilder = new RuleOutcomeDetailsBuilder(RuleId.ProfanityChecks);
        var content = await prohibitedContentRepository.GetByContentTypeAsync(ProhibitedContentType.Profanity, CancellationToken.None);
        ProfanityList = content.Select(x => x.Content);

        var outcomes = new List<RuleOutcome>();
        outcomes.AddRange(ProfanityCheckAsync(() => subject.Title));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.ShortDescription));
        if (subject.EmployerLocations is { Count: > 0 })
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.EmployerLocations.Select(x => x.Flatten()).ToDelimitedString(", "), "EmployerLocations"));
        }
        outcomes.AddRange(ProfanityCheckAsync(() => subject.EmployerLocationInformation));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.Wage!.WorkingWeekDescription));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.Wage!.WageAdditionalInformation));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.Description));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.TrainingDescription));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.OutcomeDescription));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.ThingsToConsider));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.AdditionalQuestion1));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.AdditionalQuestion2));
        if (subject.Skills != null)
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.Skills.ToDelimitedString(","), "Skills"));
        }
        if (subject.Qualifications != null)
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.Qualifications.SelectMany(q => new[] { q.Grade, q.Subject }).ToDelimitedString(","), "Qualifications"));
        }
        outcomes.AddRange(ProfanityCheckAsync(() => subject.EmployerDescription));

        
        if (subject.ProviderContact is { Name: not null })
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.ProviderContact.Name));
        }

        if (subject.EmployerContact is { Name: not null })
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.EmployerContact.Name));
        }
            
        if (subject is { EmployerNameOption: EmployerNameOption.Anonymous or EmployerNameOption.TradingName })
        {
            outcomes.AddRange(ProfanityCheckAsync(() => subject.EmployerName));
        }

        outcomes.AddRange(ProfanityCheckAsync(() => subject.ApplicationInstructions));
        outcomes.AddRange(ProfanityCheckAsync(() => subject.AdditionalTrainingDescription));

        var outcome = outcomeBuilder
            .Add(outcomes)
            .ComputeSum();

        return outcome;
    }
}