using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// The class that represents inputs for setting a boat to be at sea
    /// </summary>
    public class DepartureInputEntity
    {
        /// <summary>
        /// The departure date of the boat
        /// </summary>
        [JsonProperty("departure_date")]
        public DateTime? DepartureDate { get; set; }
    }
}
