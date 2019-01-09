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
        Task AddMessageAsync(CloudQueueMessage message, TimeSpan? timeToLive, TimeSpan? initialVisibilityDelay);
        Task<CloudQueueMessage> GetMessageAsync(TimeSpan? visibilityTimeout = null);
        Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount, TimeSpan? visibilityTimeout = null);
        Task<CloudQueueMessage> PeekMessageAsync();
        Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount);
        Task DeleteMessageAsync(CloudQueueMessage message);
        Task UpdateMessageAsync(CloudQueueMessage message, TimeSpan visibilityTimeout, MessageUpdateFields updateFields);
    }
}