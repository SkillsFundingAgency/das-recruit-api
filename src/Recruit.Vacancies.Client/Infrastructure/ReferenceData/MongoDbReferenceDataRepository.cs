using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Mongo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.Skills;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Programmes = Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.ApprenticeshipProgrammes;
using Quals = Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData.Qualifications;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.ReferenceData
{
    internal sealed class MongoDbReferenceDataRepository : MongoDbCollectionBase, IReferenceDataReader
    {
        private const string Id = "_id";
        private readonly IDictionary<Type, string> _itemIdLookup;

        public MongoDbReferenceDataRepository(ILoggerFactory loggerFactory, IOptions<MongoDbConnectionDetails> details)
            : base(loggerFactory, MongoDbNames.RecruitDb, MongoDbCollectionNames.ReferenceData, details)
        {
            _itemIdLookup = BuildLookup();
        }

        public async Task<T> GetReferenceData<T>() where T : class, IReferenceDataItem
        {
            try
            {
                var id = _itemIdLookup[typeof(T)];

                var filter = Builders<T>.Filter.Eq(Id, id);
                var collection = GetCollection<T>();

                var result = await RetryPolicy.Execute(_=>
                    collection.Find(filter).SingleOrDefaultAsync(),
                    new Context(nameof(GetReferenceData)));

                return result;
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentOutOfRangeException($"{typeof(T).Name} is not a recognised reference data type", ex);
            }
        }
        
        private IDictionary<Type, string> BuildLookup()
        {
            return new Dictionary<Type, string> 
            {
                { typeof(CandidateSkills), "CandidateSkills" },
                { typeof(Quals.Qualifications), "QualificationTypes" },
                { typeof(Programmes.ApprenticeshipProgrammes), "ApprenticeshipProgrammes" },
                { typeof(Profanities.ProfanityList), "Profanities" },
                { typeof(BannedPhrases.BannedPhraseList), "BannedPhrases" },
                { typeof(TrainingProviders.TrainingProviders), "Providers" }
            };
        }
    }
}