using Cache.DAL.Repositories.Interfaces;

namespace Cache.WEB.Settings
{
    public class CacheSettings : ICacheSettings
    {
        public int ExpirationMinutes { get; set; }

        public bool UseRefreshAheadStrategy { get; set; }

        public bool UseWriteBehindStrategy { get; set; }

        public int WriteBehindSyncInterval { get; set; }

        public int UpdateExpirationInterval { get; set; }

        public int AccessCountEnoughForUpdateExpiration { get; set; }
    }
}