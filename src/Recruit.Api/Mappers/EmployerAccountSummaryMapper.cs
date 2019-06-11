using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.Mappers
{
    public static class EmployerAccountSummaryMapper
    {
        public static EmployerAccountSummary MapFromEmployerDashboard(EmployerDashboard dashboard, string employerAccountId)
        {
            var vacancies = dashboard.Vacancies.ToList();

            return new EmployerAccountSummary
            {
                EmployerAccountId = employerAccountId,
                NoOfVacancies = vacancies.Count(),
                LegalEntityVacancySummaries = GetLegalEntities(vacancies)
            };
        }

        private static IEnumerable<LegalEntityVacancySummary> GetLegalEntities(IEnumerable<VacancySummaryProjection> vacancies)
        {
            return vacancies
                    .Where(v => v.LegalEntityId.HasValue)
                    .GroupBy(v => v.LegalEntityId)
                    .Select((grp) => new LegalEntityVacancySummary
                    {
                        LegalEntityId = grp.Key,
                        LegalEntityName = grp.First().LegalEntityName,
                        VacancyStatusCounts = grp
                                            .GroupBy(v => v.Status)
                                            .ToDictionary(
                                                            statusGrp => statusGrp.Key.ToString().ToLower(),
                                                            statusGrp => statusGrp.Count()
                                                        )
                    });
        }
    }
}