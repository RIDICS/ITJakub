using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.DataEntities.AzureTables.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace ITJakub.MobileApps.DataEntities.AzureTables.Daos
{
    public class AzureTableGenericDao<T> where T : TableEntity, new()
    {
        private readonly CloudTable m_table;

        protected AzureTableGenericDao(AzureTablesClient azureTablesClient,string tableName)
        {
            m_table = azureTablesClient.GetTableReference(tableName);
            m_table.CreateIfNotExists();
        }

        public void Create(T tableEntity)
        {
            var insertOperation = TableOperation.Insert(tableEntity);
            m_table.Execute(insertOperation);
        }

        public void CreateAll(IEnumerable<TaskEntity> tableEntities)
        {
            var batchOperation = new TableBatchOperation();
            foreach (var tableEntity in tableEntities)
            {
                batchOperation.Insert(tableEntity);
            }
            m_table.ExecuteBatch(batchOperation);
        }

        public void Delete(T tableEntity)
        {
            var deleteOperation = TableOperation.Delete(tableEntity);
            m_table.Execute(deleteOperation);
        }

        public void Delete(string rowKey, string partitionKey)
        {
            Delete(FindByRowAndPartitionKey(rowKey, partitionKey));
        }

        public IEnumerable<T> GetAllByPartitionKey(string partitionKey)
        {
            TableQuery<T> rangeQuery = new TableQuery<T>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            return m_table.ExecuteQuery(rangeQuery);
        }

        public IEnumerable<T> FindAll(IEnumerable<T> tableEntities)
        {
            return tableEntities.Select(tableEntity => FindByRowAndPartitionKey(tableEntity.RowKey, tableEntity.PartitionKey)).ToList();    //HACK TODO check if is possible to retrieve more unique results in batch
        }

        public T FindByRowAndPartitionKey(string rowKey, string partitionKey)
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);            
            return (T) m_table.Execute(retrieveOperation).Result;
        }
    }
}