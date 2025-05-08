using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewStatus
{
    New,
    PendingReview,
    UnderReview,
    Closed
}