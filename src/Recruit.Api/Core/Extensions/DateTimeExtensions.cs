namespace SFA.DAS.Recruit.Api.Core.Extensions;

public static class DateTimeExtensions
{
    public static string ToDayMonthYearString(this DateTime? date)
    {
        return date?.ToString("d MMMM yyyy") ?? string.Empty;
    }
}