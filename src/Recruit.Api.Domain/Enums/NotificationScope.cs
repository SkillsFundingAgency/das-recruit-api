using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationScope
{
    NotSet,
    UserSubmittedVacancies,
    OrganisationVacancies,
}