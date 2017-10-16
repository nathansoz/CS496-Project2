using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// Provides a response with properties that we don't want to serialize in the database
    /// </summary>
    public class BoatResponseEntity : BoatEntity
    {
        /// <summary>
        /// The self URL reference
        /// </summary>
        [JsonProperty(PropertyName = "self")]
        public string Self => $"/api/boats/{Id}";

        /// <summary>
        /// Constructor that creates the response from a boat
        /// </summary>
        /// <param name="entity"></param>
        public BoatResponseEntity(BoatEntity entity)
        {
            Name = entity.Name;
            Id = entity.Id;
            Length = entity.Length;
            AtSea = entity.AtSea;
            Type = entity.Type;
        }
    }
}
