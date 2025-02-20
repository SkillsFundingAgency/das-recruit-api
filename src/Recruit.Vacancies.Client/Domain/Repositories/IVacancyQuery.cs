using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esfa.Recruit.Vacancies.Client.Domain.Repositories;

public interface IVacancyQuery
{
    Task<Vacancy> GetVacancyAsync(long vacancyReference);
    Task<IList<Vacancy>> FindClosedVacancies(IList<long> vacancyReferences);
}