using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WageType
{
    FixedWage,
    NationalMinimumWageForApprentices,
    NationalMinimumWage,
    CompetitiveSalary
}