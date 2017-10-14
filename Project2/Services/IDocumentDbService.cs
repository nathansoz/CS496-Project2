using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Services
{
    /// <summary>
    /// Represents a service that can write data to documentdb
    /// </summary>
    public interface IDocumentDbService<T>
    {
        /// <summary>
        /// Get all items
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// Get a single item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Insert a document into the db
        /// </summary>
        /// <returns></returns>
        Task UpsertAsync(T item);

        /// <summary>
        /// Delete the given item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid item);
    }
}
