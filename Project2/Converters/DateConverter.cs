using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
