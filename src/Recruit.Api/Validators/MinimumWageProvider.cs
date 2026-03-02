using SFA.DAS.VacancyServices.Wage;

namespace SFA.DAS.Recruit.Api.Validators;

public interface IMinimumWageProvider
{
    MinimumWage GetWagePeriod(DateTime date);
}

public class MinimumWageProvider : IMinimumWageProvider
{
    public MinimumWage GetWagePeriod(DateTime date)
    {
        var minimumWages = NationalMinimumWageService.GetRatesAsync().Result;

        var wagePeriods = minimumWages.OrderBy(w => w.ValidFrom);

        MinimumWage? currentWagePeriod = null;
        foreach (var wagePeriod in wagePeriods)
        {
            if (date.Date < wagePeriod.ValidFrom)
                break;

            if (currentWagePeriod != null && currentWagePeriod.ValidFrom == wagePeriod.ValidFrom)
                throw new InvalidOperationException($"Duplicate wage period: {currentWagePeriod.ValidFrom}");

            currentWagePeriod = new MinimumWage
            {
                ValidFrom = wagePeriod.ValidFrom,
                ApprenticeshipMinimumWage = wagePeriod.ApprenticeMinimumWage,
                NationalMinimumWageLowerBound = wagePeriod.Under18NationalMinimumWage,
                NationalMinimumWageUpperBound = wagePeriod.Over25NationalMinimumWage
            };
        }

        if (currentWagePeriod == null)
            throw new InvalidOperationException("Wage period is missing");

        return currentWagePeriod;
        
    }
}
public class MinimumWage
{
    public DateTime ValidFrom { get; set; }
    public decimal ApprenticeshipMinimumWage { get; set; }
    public decimal NationalMinimumWageLowerBound { get; set; }
    public decimal NationalMinimumWageUpperBound { get; set; }
}