using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application;
using Esfa.Recruit.Vacancies.Client.Application.Validation;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi.Responses;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyAnalytics;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.VacancyApplications;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client
{
    public interface IRecruitVacancyClient
    {
        Task AssignVacancyNumber(Guid vacancyId);
        Task<Vacancy> GetVacancyAsync(Guid vacancyId);
        Task<List<string>> GetCandidateSkillsAsync();
        Task<IList<string>> GetCandidateQualificationsAsync();
        EntityValidationResult Validate(Vacancy vacancy, VacancyRuleSet rules);
        Task<EmployerProfile> GetEmployerProfileAsync(string employerAccountId, string accountLegalEntityPublicHashedId);
        Task UpdateEmployerProfileAsync(EmployerProfile employerProfile, VacancyUser user);
        Task<User> GetUsersDetailsAsync(string userId);
        Task<GetUserAccountsResponse> GetEmployerIdentifiersAsync(string userId, string email);
        
    }
}