using Newtonsoft.Json;
using Project2.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// Represents a boat departure
    /// </summary>
    public class DepartureLogEntity
    {
        /// <summary>
        /// The boat that departed
        /// </summary>
        [JsonProperty("departed_boat")]
        public Guid BoatId { get; set; }

        /// <summary>
        /// The date the boat arrived
        /// </summary>
        [JsonProperty("departure_date")]
        [JsonConverter(typeof(DateConverter))]
        public DateTime DepartureDate { get; set; }
    }
}
