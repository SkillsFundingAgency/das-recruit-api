using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProhibitedContentType
{
    BannedPhrases,
    Profanity,
}