using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.ObjectModel;
using Producer.ObjectModel;
using Services;
using Services.Queue;
using Services.Table;

namespace Consumer
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
            var cancellationTokenSource = new CancellationTokenSource();

            ConsumeMessages(cancellationTokenSource.Token, 1, 1).GetAwaiter().GetResult();

            Console.WriteLine("Pressione alguma tecla para finalizar o programa");
            Console.ReadKey();
        }

        private static async Task ConsumeMessages(CancellationToken cancellationToken, int numberOfMessages = 10, int numberOfThreads = 50)
        {
            Stopwatch time = Stopwatch.StartNew();
            try
            {
                string path = $@"c:\teste\{_startExecution:dd-MM-yyyy_hh-mm-ss}mensagensFila.txt";

                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("Iniciando execução");
                    }
                }

                SemaphoreSlim sem = new SemaphoreSlim(numberOfThreads, numberOfThreads);
                List<Task> tasks = new List<Task>();
                List<string> results = new List<string>();

                do
                {
                    for (int i = 0; i < numberOfMessages; i++)
                    {
                        await sem.WaitAsync(cancellationToken);

                        tasks.Add(_queueService.GetMessageAsync().ContinueWith(async (t) =>
                        {
                            if (t.IsCompleted)
                            {
                                if (t.Result != null)
                                {
                                    try
                                    {
                                        var operation = await _tableService.RetrieveEntityUsingPointQueryAsync<Operation>(t.Result.InsertionTime.Value.ToString("yyyyMMddHH"), t.Result.Id);
                                        operation.Processed = true;
                                        operation.Success = true;
                                        operation.TryCount = 1;
                                        await _tableService.MergeEntityAsync(operation);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }   
                                   

                                    results.Add(t.Result.AsString);
                                    await _queueService.DeleteMessageAsync(t.Result);
                                    Interlocked.Increment(ref _countMessagesProcessed);
                                }

                                sem.Release();
                            }


                        }, cancellationToken));
                    }

                    await Task.WhenAll(tasks);

                    if (results.Count > 0)
                    {
                        File.AppendAllLines(path, results);

                        //using (StreamWriter sw = new StreamWriter(path, true))
                        //{
                        //    foreach (var result in results)
                        //    {
                        //        sw.WriteLine(result);
                        //    }
                        //}
                    }
                    else
                    {
                        await Task.Delay(1000, cancellationToken);
                    }

                    results.Clear();

                    time.Stop();
                    Console.WriteLine("{0} Mensagens processadas em {1} segundos.", _countMessagesProcessed,
                        time.Elapsed.TotalSeconds.ToString());
                } while (!cancellationToken.IsCancellationRequested);


                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} --- {1}", ex.Message, ex.InnerException);
            }
        }
    }
}
