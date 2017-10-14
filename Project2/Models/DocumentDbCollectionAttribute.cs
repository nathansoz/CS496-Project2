using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// Mark as a collection for a document db
    /// </summary>
    public class DocumentDbCollectionAttribute : Attribute
    {
        /// <summary>
        /// The name of the collection
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="collectionName"></param>
        public DocumentDbCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
