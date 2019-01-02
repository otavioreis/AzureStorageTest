using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Table.ObjectModel;
using Services.Queue;
using Services.Table;

namespace Azure.Table
{
    class Program
    {
        private static readonly ITableService _tableService;
        private static readonly DateTime _startExecution;

        static Program()
        {
            _tableService = new TableService("tableteste", ConfigurationManager.AppSettings["StorageConnectionString"]);
            _startExecution = DateTime.Now;
        }
        static void Main(string[] args)
        {
            //InsereRegistro().GetAwaiter().GetResult();
            BuscaRegistro().GetAwaiter().GetResult();
        }

        static async Task InsereRegistro()
        {
            var entidade = new EntidadeTeste
            {
                Identificador = "Teste",
                Data = DateTime.UtcNow.AddHours(1),
                ListaEntidadesRelacionais = new EntidadeRelacional{Nome = "a", SobreNome = "a"}
            };

            Console.WriteLine(entidade.ID);
            Console.WriteLine(entidade.CreatedAt);
            Console.WriteLine(entidade.PartitionKey);
            Console.WriteLine(entidade.RowKey);
            await _tableService.InsertOrMergeEntityAsync(entidade);

            var outraEntidade = new OutraEntidade
            {
                OutroAtributo = "TesteEntidadeRelacional"
            };

           // await _tableService.InsertOrMergeEntityAsync(outraEntidade);

            Console.ReadKey();
        }

        static async Task BuscaRegistro()
        {
            var teste = await _tableService.RetrieveEntityUsingPointQueryAsync<EntidadeTeste>("2019010218", "d4a5f13d-de2c-4e37-81b7-50f2737d0400");
            teste.ListaEntidadesRelacionais.Nome = "otavio";
            teste.ListaEntidadesRelacionais = null;
            await _tableService.InsertOrReplaceEntityAsync(teste);
        }
    }
}
