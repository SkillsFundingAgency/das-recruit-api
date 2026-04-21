using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ManualQaOutcome
{
    Approved,
    Referred,
    Transferred,
    Blocked
}