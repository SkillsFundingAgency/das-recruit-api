using System;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Repositories;

internal sealed class MongoDbApplicationReviewRepository : MongoDbCollectionBase, IApplicationReviewRepository
{
    public MongoDbApplicationReviewRepository(ILoggerFactory loggerFactory, IOptions<MongoDbConnectionDetails> details)
        : base(loggerFactory, MongoDbNames.RecruitDb, MongoDbCollectionNames.ApplicationReviews, details)
    {
    }

    public Task CreateAsync(ApplicationReview review)
    {
        var collection = GetCollection<ApplicationReview>();
        return RetryPolicy.Execute(_ =>
                collection.InsertOneAsync(review),
            new Context(nameof(CreateAsync)));
    }

    public async Task<ApplicationReview> GetAsync(long vacancyReference, Guid candidateId)
    {
        var builder = Builders<ApplicationReview>.Filter;
        var filter = builder.Eq(r => r.VacancyReference, vacancyReference) &
                     builder.Eq(r => r.CandidateId, candidateId);

        var collection = GetCollection<ApplicationReview>();

        var result = await RetryPolicy.Execute(_ =>
                collection.Find(filter).SingleOrDefaultAsync(),
            new Context(nameof(GetAsync)));

        return result;
    }

    public Task UpdateAsync(ApplicationReview applicationReview)
    {
        var filter = Builders<ApplicationReview>.Filter.Eq(r => r.Id, applicationReview.Id);
        var collection = GetCollection<ApplicationReview>();

        return RetryPolicy.Execute(_ =>
                collection.ReplaceOneAsync(filter, applicationReview),
            new Context(nameof(UpdateAsync)));
    }
}