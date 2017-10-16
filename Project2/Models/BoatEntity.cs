using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// Represents a boat
    /// </summary>
    [DocumentDbCollection("Boats")]
    public class BoatEntity
    {
        /// <summary>
        /// The boat ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the boat
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The length of the boat
        /// </summary>
        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        /// <summary>
        /// Thhe type of boat
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Whether or not the boat is at sea
        /// </summary>
        [JsonProperty(PropertyName = "at_sea")]
        public bool AtSea { get; set; }
    }
}
