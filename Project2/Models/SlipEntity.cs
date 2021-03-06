﻿using Newtonsoft.Json;
using Project2.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// A spot for a boat to dock
    /// </summary>
    [DocumentDbCollection("Slips")]
    public class SlipEntity
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SlipEntity()
        {
            DepartureHistory = new List<DepartureLogEntity>();
        }

        /// <summary>
        /// The id of the slip
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The slip number
        /// </summary>
        [JsonProperty("number")]
        public int Number { get; set; }

        /// <summary>
        /// The currently docked boat
        /// </summary>
        [JsonProperty("current_boat")]
        public Guid? CurrentBoat { get; set; }

        /// <summary>
        /// The date the boat arrived
        /// </summary>
        [JsonProperty("arrival_date")]
        [JsonConverter(typeof(DateConverter))]
        public DateTime? ArrivalDate { get; set; }

        /// <summary>
        /// The departure history
        /// </summary>
        [JsonProperty("departure_history")]
        public List<DepartureLogEntity> DepartureHistory { get; }
    }
}
