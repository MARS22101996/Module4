using System;

namespace CachePower.DAL.Entities
{
    public class CachedEntity
    {
        public Cargo Entity { get; set; }

        public int AccessCount { get; set; }

        public DateTime? LastAccessed { get; set; }

        public Guid CacheUniqueKey { get; set; }
    }
}