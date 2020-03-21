using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NotesCosmosDB.Services.CosmosDB
{
    public class CosmosDBService : ICosmosDBService
    {
        private DocumentClient _client;
        private Uri _collectionLink;

        public CosmosDBService()
        {
            _client = new DocumentClient(new Uri(AppSettings.EndpointUri), AppSettings.PrimaryKey);
            _collectionLink = UriFactory.CreateDocumentCollectionUri(AppSettings.DatabaseName, AppSettings.CollectionName);
        }

        public async Task CreateDatabase(string databaseName)
        {
            try
            {
                await _client.CreateDatabaseIfNotExistsAsync(new Database
                {
                    Id = databaseName
                });
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Create Database Error: ", ex.Message);
            }
        }

        public async Task CreateDocumentCollection(string databaseName, string collectionName)
        {
            try
            {
                await _client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    new DocumentCollection
                    {
                        Id = collectionName
                    },
                    new RequestOptions
                    {
                        OfferThroughput = 400
                    });
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Create Document Error: ", ex.Message);
            }
        }

        public async Task<List<T>> GetItemsAsync<T>()
        {
            var items = new List<T>();

            try
            {
                var query = _client.CreateDocumentQuery<T>(_collectionLink)            
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    items.AddRange(await query.ExecuteNextAsync<T>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Get Items Error: ", ex.Message);
            }

            return items;
        }

        public async Task SaveItemAsync<T>(T item, string id, bool isNewItem = false)
        {
            try
            {
                if (isNewItem)
                {
                    await _client.CreateDocumentAsync(_collectionLink, item);
                }
                else
                {
                    await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(AppSettings.DatabaseName, AppSettings.CollectionName, id), item);
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Save Error: ", ex.Message);
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(AppSettings.DatabaseName, AppSettings.CollectionName, id));
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Delete Error: ", ex.Message);
            }
        }

        async Task DeleteDocumentCollection()
        {
            try
            {
                await _client.DeleteDocumentCollectionAsync(_collectionLink);
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Delete Document Error: ", ex.Message);
            }
        }

        async Task DeleteDatabase()
        {
            try
            {
                await _client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(AppSettings.DatabaseName));
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Delete Database Error: ", ex.Message);
            }
        }
    }
}
