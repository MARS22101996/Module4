namespace CachePower.DAL.Interfaces
{
    public interface ICacheSettings
    {
        int ExpirationMinutes { get; }

        bool UseRefreshAheadStrategy { get; }

        bool UseWriteBehindStrategy { get; }

        int WriteBehindSyncInterval { get; }

        int UpdateExpirationInterval { get; }

        int AccessCountEnoughForUpdateExpiration { get; }
    }
}