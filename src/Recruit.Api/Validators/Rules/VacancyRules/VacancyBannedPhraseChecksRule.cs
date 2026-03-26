using System.Linq.Expressions;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Enums;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Extensions;
using ProhibitedContentType = SFA.DAS.Recruit.Api.Domain.Models.ProhibitedContentType;

namespace SFA.DAS.Recruit.Api.Validators.Rules.VacancyRules;

public class BannedPhrasesData
{
    public string BannedPhrase { get; set; }
    public int Occurrences { get; set; }
}

public sealed class VacancyBannedPhraseChecksRule(IProhibitedContentRepository prohibitedContentRepository, decimal weighting = 100.0m): IRule<VacancySnapshot>
{
    private IEnumerable<string> BannedPhrases { get; set; } = new List<string>();

    private RuleOutcome CreateOutcome(int score, string narrative, string? data, string target = RuleOutcome.NoSpecificTarget)
    {
        return new RuleOutcome(RuleId.BannedPhraseChecks, (int) (score * weighting), narrative, target, null, data);
    }
    
    private IEnumerable<RuleOutcome> BannedPhraseCheck(Expression<Func<string?>> property, string? relatedFieldId = null)
    {
        var fieldId = relatedFieldId ?? property.GetQualifiedFieldId();

        var foundBannedPhrases = FindOccurrences(property);

        return foundBannedPhrases.Values.Sum() > 0
            ? CreateUnconsolidatedOutcomes(foundBannedPhrases, fieldId)
            : [CreateOutcome(0, $"No banned phrases found in '{fieldId}'", null, fieldId)];
    }
        
    private Dictionary<string, int> FindOccurrences(Expression<Func<string?>> property)
    {
        var value = property.Compile()();
        var checkValue = value.FormatForParsing();
        var foundBannedPhrases = new Dictionary<string, int>();

        foreach (var bannedPhrase in BannedPhrases)
        {
            var occurrences = checkValue.CountOccurrences(bannedPhrase);
            if (occurrences > 0)
            {
                foundBannedPhrases.Add(bannedPhrase, occurrences);
            }
        }

        return foundBannedPhrases;
    }
        
    private IEnumerable<RuleOutcome> CreateUnconsolidatedOutcomes(Dictionary<string, int> foundBannedPhrases, string fieldId)
    {
        return foundBannedPhrases.Select(foundBannedPhrase =>
        {
            var count = foundBannedPhrase.Value;
            var term = foundBannedPhrase.Key;
            var foundMessage = count > 1 ? $"found {count} times" : "found";
            var narrative = $"Banned phrase '{term}' {foundMessage} in '{fieldId}'";

            var data = JsonConvert.SerializeObject(new BannedPhrasesData { BannedPhrase = term, Occurrences = count });
            return CreateOutcome(count, narrative, data, fieldId);
        });
    }
        
    public async Task<RuleOutcome> EvaluateAsync(VacancySnapshot subject, CancellationToken cancellationToken = default)
    {
        var outcomeBuilder = new RuleOutcomeDetailsBuilder(RuleId.BannedPhraseChecks);
            
        var content = await prohibitedContentRepository.GetByContentTypeAsync(ProhibitedContentType.BannedPhrases, cancellationToken);
        BannedPhrases = content.Select(x => x.Content);

        var outcomes = new List<RuleOutcome>();
        outcomes.AddRange(BannedPhraseCheck(() => subject.Title));
        outcomes.AddRange(BannedPhraseCheck(() => subject.ShortDescription));
        if (subject.EmployerLocations is { Count: > 0 })
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.EmployerLocations.Select(x => x.Flatten()).ToDelimitedString(", "), "EmployerLocations"));
        }
        outcomes.AddRange(BannedPhraseCheck(() => subject.EmployerLocationInformation));
        outcomes.AddRange(BannedPhraseCheck(() => subject.Wage!.WorkingWeekDescription));
        outcomes.AddRange(BannedPhraseCheck(() => subject.Wage!.WageAdditionalInformation));
        outcomes.AddRange(BannedPhraseCheck(() => subject.Description));
        outcomes.AddRange(BannedPhraseCheck(() => subject.TrainingDescription));
        outcomes.AddRange(BannedPhraseCheck(() => subject.OutcomeDescription));
        outcomes.AddRange(BannedPhraseCheck(() => subject.ThingsToConsider));
        outcomes.AddRange(BannedPhraseCheck(() => subject.AdditionalQuestion1));
        outcomes.AddRange(BannedPhraseCheck(() => subject.AdditionalQuestion2));
        if (subject.Skills is not null)
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.Skills.ToDelimitedString(","), "Skills"));
        }
            
        if (subject.Qualifications is not null)
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.Qualifications.SelectMany(q => new[] { q.Grade, q.Subject }).ToDelimitedString(","), "Qualifications"));
        }
        outcomes.AddRange(BannedPhraseCheck(() => subject.EmployerDescription));

        if (subject.ProviderContact is { Name: not null })
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.ProviderContact.Name));
        }

        if (subject.EmployerContact is { Name: not null })
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.EmployerContact.Name));
        }
        
        if (subject is { EmployerNameOption: EmployerNameOption.Anonymous or EmployerNameOption.TradingName })
        {
            outcomes.AddRange(BannedPhraseCheck(() => subject.EmployerName));
        }
            
        outcomes.AddRange(BannedPhraseCheck(() => subject.ApplicationInstructions));
        outcomes.AddRange(BannedPhraseCheck(() => subject.AdditionalTrainingDescription));

        return outcomeBuilder
            .Add(outcomes)
            .ComputeSum();
    }
}