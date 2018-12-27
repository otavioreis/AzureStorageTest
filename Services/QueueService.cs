using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Services
{
    public class QueueService : IQueueService
    {
        private readonly CloudQueueClient _queueClient;
        private readonly CloudQueue _queue;

        public QueueService(string queueName, string connectionString)
        : this(queueName, CloudStorageAccount.Parse(connectionString))
        {
        }

        internal QueueService(string queueName, CloudStorageAccount storageAccount)
        {
            _queueClient = storageAccount.CreateCloudQueueClient();
            _queue = this.CreateQueueIfNotExists(_queueClient.GetQueueReference(queueName));
        }

        private CloudQueue CreateQueueIfNotExists(CloudQueue queue)
        {
            queue.CreateIfNotExists();

            return queue;
        }

        public async Task<List<CloudQueue>> ListExistingQueues(string prefix)
        {
            QueueContinuationToken token = null;
            var cloudQueueList = new List<CloudQueue>();

            do
            {
                QueueResultSegment segment = string.IsNullOrEmpty(prefix) ? await _queueClient.ListQueuesSegmentedAsync(token) : await _queueClient.ListQueuesSegmentedAsync(prefix, token);
                token = segment.ContinuationToken;
                cloudQueueList.AddRange(segment.Results);
            }
            while (token != null);

            return cloudQueueList;
        }

        public async Task AddMessageAsync(CloudQueueMessage message)
        {
            //TimeSpan.FromTicks(DateTime.Now.AddDays(5).Ticks);
            //await _queue.AddMessageAsync(message, TimeSpan.FromDays(37), TimeSpan.FromSeconds(30), null, null);
            await _queue.AddMessageAsync(message);
        }

        public async Task<CloudQueueMessage> GetMessageAsync(TimeSpan? visibilityTimeout = null)
        {
            if (visibilityTimeout == null)
                visibilityTimeout = TimeSpan.FromMinutes(5);

            return await _queue.GetMessageAsync(visibilityTimeout, null, null);
        }

        public async Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan? visibilityTimeout = null)
        {
            if (visibilityTimeout == null)
                visibilityTimeout = TimeSpan.FromMinutes(5);

            return await _queue.GetMessagesAsync(messageCount, visibilityTimeout, null, null);
        }

        public async Task<CloudQueueMessage> PeekMessageAsync()
        {
            return await _queue.PeekMessageAsync();
        }

        public async Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount)
        {
            return await _queue.PeekMessagesAsync(messageCount);
        }


        public async Task DeleteMessageAsync(CloudQueueMessage message)
        {
            await _queue.DeleteMessageAsync(message);
        }
    }
}
