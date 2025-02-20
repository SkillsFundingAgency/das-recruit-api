using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Validation;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Client;

public interface IRecruitVacancyClient
{
    Task<Vacancy> GetVacancyAsync(Guid vacancyId);
    Task<List<string>> GetCandidateSkillsAsync();
    Task<IList<string>> GetCandidateQualificationsAsync();
    EntityValidationResult Validate(Vacancy vacancy, VacancyRuleSet rules);
    Task<EmployerProfile> GetEmployerProfileAsync(string employerAccountId, string accountLegalEntityPublicHashedId);
    Task UpdateEmployerProfileAsync(EmployerProfile employerProfile, VacancyUser user);
        
}