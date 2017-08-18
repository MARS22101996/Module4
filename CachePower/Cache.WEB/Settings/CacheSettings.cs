using Cache.DAL.Repositories.Interfaces;

namespace Cache.WEB.Settings
{
    public class CacheSettings : ICacheSettings
    {
        public int ExpirationInterval { get; set; }

        public bool IsRefreshAheadStrategy { get; set; }

        public bool IsUseWriteBehindStrategy { get; set; }

        public int WriteBehindSyncInterval { get; set; }

        public int UpdateExpiration { get; set; }

        public int AccessCountLimit { get; set; }
    }
}