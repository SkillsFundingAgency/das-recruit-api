using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.Core.Extensions;

public static class VacancyExtensions
{
    public static string GetWageDuration(this VacancyEntity vacancy)
    {
        if (vacancy.Wage_DurationUnit is null) return string.Empty;

        return vacancy.Wage_DurationUnit switch
        {
            DurationUnit.Year => Pluralize(vacancy.Wage_Duration, "year"),
            DurationUnit.Week => Pluralize(vacancy.Wage_Duration, "week"),
            DurationUnit.Month => FormatMonths(vacancy.Wage_Duration),
            _ => string.Empty
        };
    }

    private static string Pluralize(int? value, string unit)
    {
        return value is 1 ? $"1 {unit}" : $"{value} {unit}s";
    }

    private static string FormatMonths(int? totalMonths)
    {
        if (totalMonths == null) return string.Empty;

        var years = totalMonths / 12;
        var months = totalMonths % 12;

        var parts = new List<string>();

        if (years > 0) parts.Add(Pluralize(years, "year"));
        if (months > 0) parts.Add(Pluralize(months, "month"));

        return parts.Count > 0 ? string.Join(" ", parts) : string.Empty;
    }
}