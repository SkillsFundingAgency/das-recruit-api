using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using MongoDB.Driver;

namespace Esfa.Recruit.Vacancies.Client.Domain.Repositories
{
    public interface IApplicationReviewRepository
    {
        Task<ApplicationReview> GetAsync(long vacancyReference, Guid candidateId);
        Task UpdateAsync(ApplicationReview applicationReview);
    }
}
