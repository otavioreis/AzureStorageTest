using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Services.Queue
{
    public interface IQueueService
    {
        Task<List<CloudQueue>> ListExistingQueues(string prefix);
        Task AddMessageAsync(CloudQueueMessage message);
        Task<CloudQueueMessage> GetMessageAsync(TimeSpan? visibilityTimeout = null);
        Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan? visibilityTimeout = null);
        Task<CloudQueueMessage> PeekMessageAsync();
        Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount);
        Task DeleteMessageAsync(CloudQueueMessage message);
    }
}