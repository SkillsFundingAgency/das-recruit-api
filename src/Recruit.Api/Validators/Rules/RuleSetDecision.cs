using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Validators.Rules;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RuleSetDecision
{
    Unknown = 0,
    Refer,
    Approve,
    Indeterminate
}