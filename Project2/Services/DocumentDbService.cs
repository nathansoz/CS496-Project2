using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Threading;
using Microsoft.Azure.Documents.Linq;

namespace Project2.Services
{
    /// <summary>
    /// Represents a service that can write data to documentDB
    /// </summary>
    public class DocumentDbService<T> : IDocumentDbService<T>
    {
        DocumentClient _client;

        string _database;

        string _collectionId;

        Uri _collectionUri;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DocumentDbService(Uri connectionEndpoint, string key, string database, string collection)
        {
            _client = new DocumentClient(connectionEndpoint, key);
            _database = database;
            _collectionId = collection;

            Initialize().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_database, _collectionId, id.ToString()));
        }

        /// <summary>
        /// Get all of Type T
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAsync()
        {
            var ret = new List<T>();

            var query = _client.CreateDocumentQuery<T>(_collectionUri, new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

            while(query.HasMoreResults)
            {
                ret.AddRange(await query.ExecuteNextAsync<T>());
            }

            return ret;
        }

        /// <summary>
        /// Get a specific document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(Guid id)
        {
            return await _client.ReadDocumentAsync<T>(UriFactory.CreateDocumentUri(_database, _collectionId, id.ToString()));
        }

        /// <summary>
        /// Create the database if needed
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var db = await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = _database });
            var collection = await _client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_database), new DocumentCollection { Id = _collectionId }, new RequestOptions { OfferThroughput = 400 });

            _collectionUri = UriFactory.CreateDocumentCollectionUri(_database, _collectionId);
        }

        /// <summary>
        /// Insert into the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task UpsertAsync(T item)
        {
            await _client.UpsertDocumentAsync(_collectionUri, item);
        }
    }
}
