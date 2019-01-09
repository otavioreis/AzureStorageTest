using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.ObjectModel;
using Microsoft.WindowsAzure.Storage.Queue;
using Services;
using Services.Queue;
using Services.Table;

namespace Producer
{
    class Program
    {
        private static readonly IQueueService _queueService;
        private static readonly ITableService _tableService;
        private static readonly DateTime _startExecution;

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

            ProduceMessages(1, 100).GetAwaiter().GetResult();
        }

        private static async Task ProduceMessages(int numberOfMessages = 100, int numberOfThreads = 5)
        {
            Stopwatch time = Stopwatch.StartNew();
            try
            {
                int countMensagensEnviadas = 0;

                SemaphoreSlim sem = new SemaphoreSlim(numberOfThreads, numberOfThreads);
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < numberOfMessages; i++)
                {
                    Console.WriteLine($"Enviando mensagem {i + 1} para a fila.");

                    await sem.WaitAsync();

                    var content = $"{_startExecution:dd-MM-yyyy_HH-mm-ss} - {i}";
                    var message = new CloudQueueMessage(content);

                    tasks.Add(_queueService.AddMessageAsync(message, TimeSpan.MaxValue, TimeSpan.FromMinutes(30)).ContinueWith(async (t) =>
                    {
                        var operationObj = new Operation(message.Id, message.NextVisibleTime, content, message.PopReceipt);
                        await _tableService.InsertOrMergeEntityAsync(operationObj);
                        sem.Release();
                        Interlocked.Increment(ref countMensagensEnviadas);
                    }));
                }

                await Task.WhenAll(tasks);

                time.Stop();
                Console.WriteLine("{0} Mensagens enviadas em {1} segundos. Pressione alguma tecla para continuar", countMensagensEnviadas, time.Elapsed.TotalSeconds.ToString());

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
