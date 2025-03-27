using System.Text.Json.Serialization;

namespace Recruit.Api.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OwnerType
{
    Employer = 0,
    Provider = 1
}