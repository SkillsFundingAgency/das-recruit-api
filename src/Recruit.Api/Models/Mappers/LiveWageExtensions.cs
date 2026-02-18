using SFA.DAS.VacancyServices.Wage;
using WageType = SFA.DAS.Recruit.Api.Domain.Enums.WageType;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class LiveWageExtensions
{
    public static string ToDisplayText(this Wage wage, DateTime? expectedStartDate)
    {
        var wageDetails = new WageDetails
        {
            Amount = wage.FixedWageYearlyAmount,
            HoursPerWeek = wage.WeeklyHours,
            StartDate = expectedStartDate.GetValueOrDefault()
        };
        string wageText;

        switch (wage.WageType)
        {
            case WageType.FixedWage:
                wageText = WagePresenter
                    .GetDisplayText(SFA.DAS.VacancyServices.Wage.WageType.Custom, WageUnit.Annually, wageDetails)
                    .AsWholeMoneyAmount()
                    .AsPerYear();
                break;
            case WageType.NationalMinimumWage:
                wageText = WagePresenter
                    .GetDisplayText(SFA.DAS.VacancyServices.Wage.WageType.NationalMinimum, WageUnit.Annually, wageDetails)
                    .AsWholeMoneyAmount()
                    .AsPerYear();
                break;
            case WageType.NationalMinimumWageForApprentices:
                wageText = WagePresenter
                    .GetDisplayText(SFA.DAS.VacancyServices.Wage.WageType.ApprenticeshipMinimum, WageUnit.Annually, wageDetails)
                    .AsWholeMoneyAmount()
                    .AsPerYear();
                break;
            case WageType.CompetitiveSalary:
                wageText = "Competitive";
                break;
            default:
                wageText = wage.WageType.ToString();
                break;
        }

        return wageText;
    }
    public static string AsWholeMoneyAmount(this string value)
    {
        return value.Replace(".00", "");
    }
    private static string AsPerYear(this string value)
    {
        return $"{value} a year";
    }
}