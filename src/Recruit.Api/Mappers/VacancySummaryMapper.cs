using Microsoft.Extensions.Options;
using SFA.DAS.Recruit.Api.Configuration;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.Mappers
{
    public class VacancySummaryMapper : IVacancySummaryMapper
    {
        private readonly RecruitConfiguration _recruitConfig;

        public VacancySummaryMapper(IOptions<RecruitConfiguration> recruitConfig)
        {
            _recruitConfig = recruitConfig.Value;
        }

        public VacancySummary MapFromVacancySummaryProjection(VacancySummaryProjection vsp, bool isForProviderOwnedVacancies)
        {
            var raaManageVacancyFormattedUrl = isForProviderOwnedVacancies
                                                ? _recruitConfig.ProviderRecruitAnApprenticeManageVacancyFormattedUrl
                                                : _recruitConfig.EmployerRecruitAnApprenticeManageVacancyFormattedUrl;
            return new VacancySummary
            {
                EmployerAccountId = vsp.EmployerAccountId,
                Title = vsp.Title,
                VacancyReference = vsp.VacancyReference,
                LegalEntityId = vsp.LegalEntityId,
                LegalEntityName = vsp.LegalEntityName,
                EmployerName = vsp.EmployerName,
                Ukprn = vsp.Ukprn,
                CreatedDate = vsp.CreatedDate,
                Status = vsp.Status.ToString(),
                ClosingDate = vsp.ClosingDate,
                Duration = vsp.Duration,
                DurationUnit = vsp.DurationUnit.ToString(),
                ApplicationMethod = vsp.ApplicationMethod?.ToString(),
                ProgrammeId = vsp.ProgrammeId,
                StartDate = vsp.StartDate,
                TrainingTitle = vsp.TrainingTitle,
                TrainingType = vsp.TrainingType.ToString(),
                TrainingLevel = vsp.TrainingLevel.ToString(),
                NoOfNewApplications = vsp.NoOfNewApplications,
                NoOfSuccessfulApplications = vsp.NoOfSuccessfulApplications,
                NoOfUnsuccessfulApplications = vsp.NoOfUnsuccessfulApplications,

                FaaVacancyDetailUrl = $"{_recruitConfig.FindAnApprenticeshipDetailPrefixUrl}{vsp.VacancyReference}",
                RaaManageVacancyUrl = isForProviderOwnedVacancies
                                        ? string.Format(raaManageVacancyFormattedUrl, vsp.Ukprn, vsp.Id)
                                        : string.Format(raaManageVacancyFormattedUrl, vsp.EmployerAccountId, vsp.Id)
            };
        }
    }
}