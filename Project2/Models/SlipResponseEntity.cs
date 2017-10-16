using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    /// <summary>
    /// Response entity containing things we don't want to serialze to the database
    /// </summary>
    public class SlipResponseEntity : SlipEntity
    {
        /// <summary>
        /// The self URL reference
        /// </summary>
        [JsonProperty(PropertyName = "self")]
        public string Self => $"/slips/{Id}";

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entity"></param>
        public SlipResponseEntity(SlipEntity entity)
        {
            Id = entity.Id;
            Number = entity.Number;
            ArrivalDate = entity.ArrivalDate;
            CurrentBoat = entity.CurrentBoat;
        }
    }
}
