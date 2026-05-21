using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Validators.Rules;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RuleId
{
    ProfanityChecks,
    BannedPhraseChecks,
    VacancyAnonymous
}