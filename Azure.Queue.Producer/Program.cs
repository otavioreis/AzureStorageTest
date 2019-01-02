using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Services;
using Services.Queue;

namespace Producer
{
    class Program
    {
        private static readonly IQueueService _queueService;
        private static readonly DateTime _startExecution;

        static Program()
        {
            _queueService = new QueueService("otavioteste", ConfigurationManager.AppSettings["StorageConnectionString"]);
            _startExecution = DateTime.Now;
        }

        static void Main(string[] args)
        {
            //Variaveis
            ThreadPool.SetMinThreads(100, 4);
            ServicePointManager.DefaultConnectionLimit = 100;

            ProduceMessages(1, 1).GetAwaiter().GetResult();
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

                    var message = new CloudQueueMessage($"{_startExecution:dd-MM-yyyy_HH-mm-ss} - {i}");
                    tasks.Add(_queueService.AddMessageAsync(message).ContinueWith((t) =>
                    {
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
