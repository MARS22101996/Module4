using System;

namespace Cache.DAL.Entities
{
    public class CachedCargo
    {
		public int AccessCount { get; set; }

		public DateTime? LastAccessed { get; set; }

		public Cargo EntityCargo { get; set; }

        public Guid Key { get; set; }
    }
}