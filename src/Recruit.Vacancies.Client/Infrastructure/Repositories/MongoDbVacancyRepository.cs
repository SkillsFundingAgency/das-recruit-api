using System;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Esfa.Recruit.Vacancies.Client.Application.Exceptions;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Exceptions;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Esfa.Recruit.Vacancies.Client.Domain.Models;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Repositories
{
    internal class MongoDbVacancyRepository : MongoDbCollectionBase, IVacancyRepository, IVacancyQuery
    {
        private const string EmployerAccountIdFieldName = "employerAccountId";
        private const string ProviderUkprnFieldName = "trainingProvider.ukprn";
        private const string OwnerTypeFieldName = "ownerType";
        private const string IsDeletedFieldName = "isDeleted";
        private const string VacancyStatusFieldName = "status";
        private const string VacancyReferenceFieldName = "vacancyReference";
        private const string CreatedByUserId = "createdByUser.userId";
        private const string SubmittedByUserId = "submittedByUser.userId";

        public MongoDbVacancyRepository(ILoggerFactory loggerFactory, IOptions<MongoDbConnectionDetails> details)
            : base(loggerFactory, MongoDbNames.RecruitDb, MongoDbCollectionNames.Vacancies, details)
        {
        }

        public Task CreateAsync(Vacancy vacancy)
        {
            var collection = GetCollection<Vacancy>();
            return RetryPolicy.Execute(_ =>
                collection.InsertOneAsync(vacancy),
                new Context(nameof(CreateAsync)));
        }

        public async Task<Vacancy> GetVacancyAsync(long vacancyReference)
        {
            var vacancy = await FindVacancy(v => v.VacancyReference, vacancyReference);

            if (vacancy == null)
                throw new VacancyNotFoundException(string.Format(ExceptionMessages.VacancyWithReferenceNotFound, vacancyReference));

            return vacancy;
        }

        public async Task<Vacancy> GetVacancyAsync(Guid id)
        {
            var vacancy = await FindVacancy(v => v.Id, id);

            if (vacancy == null)
                throw new VacancyNotFoundException(string.Format(ExceptionMessages.VacancyWithIdNotFound, id));

            return vacancy;
        }

        private async Task<Vacancy> FindVacancy<TField>(Expression<Func<Vacancy, TField>> expression, TField value)
        {
            var filter = Builders<Vacancy>.Filter.Eq(expression, value);
            var collection = GetCollection<Vacancy>();

            var result = await RetryPolicy.Execute(async _ =>
                await collection.Find(filter).SingleOrDefaultAsync(),
                new Context(nameof(FindVacancy)));

            return result;
        }

        public async Task<IList<Vacancy>> FindClosedVacancies(IList<long> vacancyReferences)
        {
            var collection = GetCollection<Vacancy>();
            var builderFilter = Builders<Vacancy>.Filter;
            var filter =  builderFilter.In<string>("status", new List<string>{VacancyStatus.Closed.ToString(),VacancyStatus.Live.ToString()}) 
                          & builderFilter.In<long>(identifier => identifier.VacancyReference.Value, vacancyReferences);
            
            var result = await RetryPolicy.Execute(async _ =>
                    await collection.Find(filter).ToListAsync(),
                new Context(nameof(FindVacancy)));

            return result;
        }

        public async Task<IEnumerable<T>> GetVacanciesByStatusAsync<T>(VacancyStatus status)
        {
            var filter = Builders<T>.Filter.Eq(VacancyStatusFieldName, status.ToString());

            var collection = GetCollection<T>();

            var result = await RetryPolicy.Execute(_ =>
                collection.Find(filter)
                .Project<T>(GetProjection<T>())
                .ToListAsync(),
            new Context(nameof(GetVacanciesByStatusAsync)));

            return result;
        }

        public async Task UpdateAsync(Vacancy vacancy)
        {
            var filter = Builders<Vacancy>.Filter.Eq(v => v.Id, vacancy.Id);
            var collection = GetCollection<Vacancy>();
            await RetryPolicy.Execute(async _ =>
                await collection.ReplaceOneAsync(filter, vacancy),
                new Context(nameof(UpdateAsync)));
        }
    }
}