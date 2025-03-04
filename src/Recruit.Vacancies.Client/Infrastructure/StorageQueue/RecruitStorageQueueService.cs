using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Application.Queues;
using Esfa.Recruit.Vacancies.Client.Application.Queues.Messages;
using Esfa.Recruit.Vacancies.Client.Infrastructure.EventStore;
using Microsoft.WindowsAzure.Storage;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.StorageQueue
{
    internal class RecruitStorageQueueService : StorageQueueServiceBase, IRecruitQueueService
    {
        private readonly Dictionary<Type, string> _messageToStorageQueueMapper = new Dictionary<Type, string>
        {
            { typeof(EventItem), QueueNames.DomainEventsQueueName }
        };

        protected override string ConnectionString { get; }

        public RecruitStorageQueueService(string connString)
        {
            ConnectionString = connString;
        }

        public override async Task AddMessageAsync<T>(T message)
        {
            var queueName = _messageToStorageQueueMapper[typeof(T)];

            if(string.IsNullOrEmpty(queueName))
                throw new InvalidEnumArgumentException($"Cannot map type {typeof(T).Name} to a queue name");

            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var client = storageAccount.CreateCloudQueueClient();

            var queue = client.GetQueueReference(queueName);

            await AddMessageToQueueAsync(queue, message);
        }
    }
}
