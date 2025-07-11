using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VacancyStatus
{
    Draft,
    Review,
    Rejected,
    Submitted,
    Referred,
    Live,
    Closed,
    Approved
}