using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.Table;

namespace Services.Table
{
    public interface ITableService
    {
        Task DeleteEntityAsync<T>(T deleteEntity) where T : ITableEntity;
        Task<T> InsertEntityAsync<T>(T entity) where T : ITableEntity;
        Task<T> InsertOrMergeEntityAsync<T>(T entity) where T : ITableEntity;
        Task<T> InsertOrReplaceEntityAsync<T>(T entity) where T : ITableEntity;
        Task<T> MergeEntityAsync<T>(T entity) where T : ITableEntity;
        Task<T> ReplaceEntityAsync<T>(T entity) where T : ITableEntity;
        Task<T> RetrieveEntityUsingPointQueryAsync<T>(string partitionKey, string rowKey) where T : ITableEntity;
    }
}