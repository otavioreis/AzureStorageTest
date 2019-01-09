using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.ObjectModel;
using Microsoft.WindowsAzure.Storage.Queue;
using Services.Queue;
using Services.Table;

namespace Azure.Queue.OtherTests
{
    class Program
    {
        private static readonly IQueueService _queueService;
        private static readonly ITableService _tableService;
        private static readonly DateTime _startExecution;
        private static int _countMessagesProcessed;

        static Program()
        {
            _queueService = new QueueService("otavioteste", ConfigurationManager.AppSettings["StorageConnectionString"]);
            _tableService = new TableService("tableteste", ConfigurationManager.AppSettings["StorageConnectionString"]);
            _startExecution = DateTime.Now;
        }

        static void Main(string[] args)
        {
            //Variaveis
            ThreadPool.SetMinThreads(100, 4);
            ServicePointManager.DefaultConnectionLimit = 100;

            UpdateMessage().GetAwaiter().GetResult();
        }

        private static async Task UpdateMessage()
        {
            var operation = await _tableService.RetrieveEntityUsingPointQueryAsync<Operation>("2019010918", "15034a7b-bf0a-4d82-a161-9c8f6d8f1009");

            CloudQueueMessage message = new CloudQueueMessage(operation.ID.ToString(), operation.PopReceipt);
            message.SetMessageContent("teste3");

            await _queueService.UpdateMessageAsync(message, TimeSpan.FromMinutes(0), MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            operation.PopReceipt = message.PopReceipt;
            await _tableService.MergeEntityAsync(operation);
        }
    }
}
