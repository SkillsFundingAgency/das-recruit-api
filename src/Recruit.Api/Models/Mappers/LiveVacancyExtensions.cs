using SFA.DAS.VacancyServices.Wage;
using WageType = SFA.DAS.Recruit.Api.Domain.Enums.WageType;

namespace SFA.DAS.Recruit.Api.Models.Mappers;

public static class LiveVacancyExtensions
{
    public static void AddWageData(this Vacancy vacancy)
    {
        if (vacancy.Wage?.WageType != null)
        {
            var wage = new Wage
            {
                FixedWageYearlyAmount = vacancy.Wage.FixedWageYearlyAmount,
                WageType = vacancy.Wage.WageType,
                WeeklyHours = vacancy.Wage.WeeklyHours
            };

            vacancy.Wage.WageText = wage.ToDisplayText(vacancy.StartDate);
        }

        if (vacancy.Wage.WageType == WageType.FixedWage)
        {
            vacancy.Wage.ApprenticeMinimumWage = vacancy.Wage.FixedWageYearlyAmount;
            vacancy.Wage.Under18NationalMinimumWage = vacancy.Wage.FixedWageYearlyAmount;
            vacancy.Wage.Between18AndUnder21NationalMinimumWage = vacancy.Wage.FixedWageYearlyAmount;
            vacancy.Wage.Between21AndUnder25NationalMinimumWage = vacancy.Wage.FixedWageYearlyAmount;
            vacancy.Wage.Over25NationalMinimumWage = vacancy.Wage.FixedWageYearlyAmount;
        }
        else if (vacancy.Wage.WageType == WageType.NationalMinimumWageForApprentices)
        {
            var rates = NationalMinimumWageService.GetAnnualRates(vacancy.StartDate!.Value, vacancy.Wage.WeeklyHours!.Value);

            vacancy.Wage.ApprenticeMinimumWage = rates.ApprenticeMinimumWage;
            vacancy.Wage.Under18NationalMinimumWage = rates.ApprenticeMinimumWage;
            vacancy.Wage.Between18AndUnder21NationalMinimumWage = rates.ApprenticeMinimumWage;
            vacancy.Wage.Between21AndUnder25NationalMinimumWage = rates.ApprenticeMinimumWage;
            vacancy.Wage.Over25NationalMinimumWage = rates.ApprenticeMinimumWage;
        }
        else
        {
            var rates = NationalMinimumWageService.GetAnnualRates(vacancy.StartDate!.Value, vacancy.Wage.WeeklyHours!.Value);

            vacancy.Wage.ApprenticeMinimumWage = rates.ApprenticeMinimumWage;
            vacancy.Wage.Under18NationalMinimumWage = rates.Under18NationalMinimumWage;
            vacancy.Wage.Between18AndUnder21NationalMinimumWage = rates.Between18AndUnder21NationalMinimumWage;
            vacancy.Wage.Between21AndUnder25NationalMinimumWage = rates.Between21AndUnder25NationalMinimumWage;
            vacancy.Wage.Over25NationalMinimumWage = rates.Over25NationalMinimumWage;
        }
    }
}