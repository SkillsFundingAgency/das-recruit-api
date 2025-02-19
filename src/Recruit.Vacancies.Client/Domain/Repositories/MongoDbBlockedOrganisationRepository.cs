using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Entities;
using Esfa.Recruit.Vacancies.Client.Domain.Repositories;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Repositories
{
    internal sealed class MongoDbBlockedOrganisationRepository : MongoDbCollectionBase, IBlockedOrganisationQuery
    {
        public MongoDbBlockedOrganisationRepository(ILoggerFactory loggerFactory, IOptions<MongoDbConnectionDetails> details)
            : base(loggerFactory, MongoDbNames.RecruitDb, MongoDbCollectionNames.BlockedOrganisations, details)
        {
        }

        public async Task<BlockedOrganisation> GetByOrganisationIdAsync(string organisationId)
        {
            var builder = Builders<BlockedOrganisation>.Filter;
            var filter = builder.Eq(bo => bo.OrganisationId, organisationId);
            var collection = GetCollection<BlockedOrganisation>();

            var result = await RetryPolicy.Execute(_ =>
                collection.Find(filter)
                    .SortByDescending(bo => bo.UpdatedDate).FirstOrDefaultAsync(),
            new Context(nameof(GetByOrganisationIdAsync)));

            return result;
        }
    }
}