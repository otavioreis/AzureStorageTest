using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;

namespace Services.Table
{
    public class TableService : ITableService
    {
        private readonly CloudTableClient _tableClient;
        private readonly CloudTable _table;

        public TableService(string tableName, string connectionString) :
            this(tableName, CloudStorageAccount.Parse(connectionString))
        {
        }

        internal TableService(string tableName, CloudStorageAccount storageAccount)
        {
            _tableClient = storageAccount.CreateCloudTableClient();
            _table = this.CreateTableIfNotExists(tableName);
        }

        private CloudTable CreateTableIfNotExists(string tableName)
        {
            var table = _tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();

            return table;
        }

        public async Task<T> RetrieveEntityUsingPointQueryAsync<T>(string partitionKey, string rowKey) where T : ITableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = await _table.ExecuteAsync(retrieveOperation);
            T entity = (T)result.Result;

            return entity;
        }

        public async Task<T> InsertEntityAsync<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableOperation insertOrMergeOperation = TableOperation.Insert(entity);

            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            T insertedEntity = (T)result.Result;

            return insertedEntity;
        }

        public async Task<T> InsertOrReplaceEntityAsync<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrReplace(entity);

            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            T insertedEntity = (T)result.Result;

            return insertedEntity;
        }

        public async Task<T> InsertOrMergeEntityAsync<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            T insertedEntity = (T)result.Result;

            return insertedEntity;
        }

        public async Task<T> MergeEntityAsync<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableOperation insertOrMergeOperation = TableOperation.Merge(entity);

            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            T insertedEntity = (T)result.Result;

            return insertedEntity;
        }

        public async Task<T> ReplaceEntityAsync<T>(T entity) where T : ITableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableOperation insertOrMergeOperation = TableOperation.Replace(entity);

            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
            T insertedEntity = (T)result.Result;

            return insertedEntity;
        }

        public async Task DeleteEntityAsync<T>(T deleteEntity) where T : ITableEntity
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException(nameof(deleteEntity));
            }

            TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
            await _table.ExecuteAsync(deleteOperation);
        }
    }
}
