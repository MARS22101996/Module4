using System;
using System.Collections.Generic;
using System.Linq;
using CachePower.DAL.Entities;
using CachePower.DAL.Exceptions;
using CachePower.DAL.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CachePower.DAL.Repositories
{
    public class CacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : BaseType, new()
    {
        private readonly IServer _server;
        private readonly IDatabase _database;
        private readonly ICacheSettings _settings;

        public CacheRepository(IServer server, IDatabase database, ICacheSettings settings)
        {
            _server = server;
            _database = database;
            _settings = settings;
        }

        public void Set(TEntity entity)
        {
            var key = GenerateKey(entity);
            var cachedEntity = GenerateCacheEntity(entity);

            SetCachedEntity(key, cachedEntity);
        }

        public CachedEntity<TEntity> Get(int id)
        {
            CachedEntity<TEntity> result = null;

            var key = GenerateKey(new TEntity { Id = id });
            var cachedValue = _database.StringGet(key);

            if (cachedValue.HasValue)
            {
                result = JsonConvert.DeserializeObject<CachedEntity<TEntity>>(cachedValue);
                result.AccessCount++;
                result.LastAccessed = DateTime.UtcNow;

                UpdateCachedEntity(key, result);
            }

            return result;
        }

        public IEnumerable<CachedEntity<TEntity>> PopAllCreated()
        {
            if (!_settings.UseWriteBehindStrategy)
            {
                throw new CacheException("Write behind strategy should be anabled to pop cache marked as created");
            }

            var result = new List<CachedEntity<TEntity>>();

            var cachedValues = _database.SetScan("Create_" + typeof(TEntity).Name).ToList();

            while (cachedValues.Any())
            {
                result.AddRange(cachedValues
                    .Select(value => JsonConvert.DeserializeObject<CachedEntity<TEntity>>(value)));

                _database.SetRemove("Create_" + typeof(TEntity).Name, cachedValues.ToArray());
                cachedValues = _database.SetScan("Create_" + typeof(TEntity).Name).ToList();
            }

            return result;
        }

        public void SetAsCreated(TEntity entity)
        {
            if (!_settings.UseWriteBehindStrategy)
            {
                throw new CacheException("Write behind strategy should be anabled to set cache as created");
            }

            var key = "Create_" + typeof(TEntity).Name;
            var cachedEntity = GenerateCacheEntity(entity);

            var value = JsonConvert.SerializeObject(cachedEntity);

            _database.SetAdd(key, value);
        }

        public IEnumerable<CachedEntity<TEntity>> GetAll()
        {
            var result = new List<CachedEntity<TEntity>>();

            var keys = _server.Keys(pattern: typeof(TEntity).Name + "_*");
            var cachedValues = _database.StringGet(keys.ToArray());

            foreach (var value in cachedValues)
            {
                if (value.HasValue)
                {
                    result.Add(JsonConvert.DeserializeObject<CachedEntity<TEntity>>(value));
                }
            }

            return result;
        }

        private static CachedEntity<TEntity> GenerateCacheEntity(TEntity entity)
        {
            var cachedEntity = new CachedEntity<TEntity>
            {
                Entity = entity,
                AccessCount = 1,
                CacheUniqueKey = Guid.NewGuid()
            };

            return cachedEntity;
        }

        private void SetCachedEntity(string key, CachedEntity<TEntity> cachedEntity)
        {
            _database.StringSet(
                key,
                JsonConvert.SerializeObject(cachedEntity),
                TimeSpan.FromMinutes(_settings.ExpirationMinutes));
        }

        private void UpdateCachedEntity(string key, CachedEntity<TEntity> cachedEntity)
        {
            var expiry = _database.StringGetWithExpiry(key).Expiry;

            _database.StringSet(key, JsonConvert.SerializeObject(cachedEntity), expiry);
        }

        private string GenerateKey(TEntity entity)
        {
            var key = typeof(TEntity).Name + "_" + entity.Id;

            return key;
        }
    }
}