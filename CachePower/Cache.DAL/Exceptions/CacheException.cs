using System;

namespace CachePower.DAL.Exceptions
{
    public class CacheException : Exception
    {
        public CacheException(string message) : base(message)
        {
        }

        public CacheException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}