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
    public class CacheRepository : ICacheRepository
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

        public void Set(Cargo entity)
        {
            var key = GenerateKey(entity);
            var cachedEntity = GenerateCacheEntity(entity);

            SetCachedEntity(key, cachedEntity);
        }

        public CachedEntity Get(int id)
        {
            CachedEntity result = null;

            var key = GenerateKey(new Cargo{ Id = id });
            var cachedValue = _database.StringGet(key);

            if (cachedValue.HasValue)
            {
                result = JsonConvert.DeserializeObject<CachedEntity>(cachedValue);
                result.AccessCount++;
                result.LastAccessed = DateTime.UtcNow;

                UpdateCachedEntity(key, result);
            }

            return result;
        }

        public IEnumerable<CachedEntity> PopAllCreated()
        {
            if (!_settings.UseWriteBehindStrategy)
            {
                throw new CacheException("Write behind strategy should be anabled to pop cache marked as created");
            }

            var result = new List<CachedEntity>();

            var cachedValues = _database.SetScan("Create_" + typeof(Cargo).Name).ToList();

            while (cachedValues.Any())
            {
                result.AddRange(cachedValues
                    .Select(value => JsonConvert.DeserializeObject<CachedEntity>(value)));

                _database.SetRemove("Create_" + typeof(Cargo).Name, cachedValues.ToArray());
                cachedValues = _database.SetScan("Create_" + typeof(Cargo).Name).ToList();
            }

            return result;
        }

        public void SetAsCreated(Cargo entity)
        {
            if (!_settings.UseWriteBehindStrategy)
            {
                throw new CacheException("Write behind strategy should be anabled to set cache as created");
            }

            var key = "Create_" + typeof(Cargo).Name;
            var cachedEntity = GenerateCacheEntity(entity);

            var value = JsonConvert.SerializeObject(cachedEntity);

            _database.SetAdd(key, value);
        }

        public IEnumerable<CachedEntity> GetAll()
        {
            var result = new List<CachedEntity>();

            var keys = _server.Keys(pattern: typeof(Cargo).Name + "_*");
            var cachedValues = _database.StringGet(keys.ToArray());

            foreach (var value in cachedValues)
            {
                if (value.HasValue)
                {
                    result.Add(JsonConvert.DeserializeObject<CachedEntity>(value));
                }
            }

            return result;
        }

        private static CachedEntity GenerateCacheEntity(Cargo entity)
        {
            var cachedEntity = new CachedEntity
            {
                Entity = entity,
                AccessCount = 1,
                CacheUniqueKey = Guid.NewGuid()
            };

            return cachedEntity;
        }

        private void SetCachedEntity(string key, CachedEntity cachedEntity)
        {
            _database.StringSet(
                key,
                JsonConvert.SerializeObject(cachedEntity),
                TimeSpan.FromMinutes(_settings.ExpirationMinutes));
        }

        private void UpdateCachedEntity(string key, CachedEntity cachedEntity)
        {
            var expiry = _database.StringGetWithExpiry(key).Expiry;

            _database.StringSet(key, JsonConvert.SerializeObject(cachedEntity), expiry);
        }

        private string GenerateKey(Cargo entity)
        {
            var key = typeof(Cargo).Name + "_" + entity.Id;

            return key;
        }
    }
}