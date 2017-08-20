namespace Cache.DAL.Repositories.Interfaces
{
    public interface ICacheSettings
    {
        int ExpirationInterval { get; }

        bool IsRefreshAheadStrategy { get; }

        bool IsUseWriteBehindStrategy { get; }

        int WriteBehindSyncInterval { get; }

        int UpdateExpiration { get; }

        int AccessCountLimit { get; }
    }
}