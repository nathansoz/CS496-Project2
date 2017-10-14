﻿using System;
using System.Runtime.Serialization;

namespace Project2
{
    [Serializable]
    internal class StartupException : Exception
    {
        public StartupException()
        {
        }

        public StartupException(string message) : base(message)
        {
        }

        public StartupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StartupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}