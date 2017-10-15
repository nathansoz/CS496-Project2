using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project2.Converters
{
    /// <summary>
    /// Convert datetime object properly when serializing
    /// From: https://stackoverflow.com/questions/18635599/specifying-a-custom-datetime-format-when-serializing-with-json-net
    /// </summary>
    public class DateConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// This constructor sets how we will serialize out the date
        /// </summary>
        public DateConverter()
        {
            DateTimeFormat = "MM/dd/yyyy";
        }

        /// <summary>
        /// Default behavior for date. We only care about writing matching the new formatting
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.Parse((string)reader.Value);
        }
    }
}
