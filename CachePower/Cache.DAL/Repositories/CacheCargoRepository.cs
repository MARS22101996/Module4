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
    public class CacheCargoRepository : ICacheCargoRepository
    {
        private readonly IServer _server;
        private readonly IDatabase _database;
        private readonly ICacheSettings _settings;
	    private const string KeyHeader = "Cargo";
	    private const string ErrorMessage = "This action suits another strategy";


		public CacheCargoRepository(IServer server, IDatabase database, ICacheSettings settings)
        {
            _server = server;
            _database = database;
            _settings = settings;
        }

        public void Configure(Cargo entity)
        {
            var key = ConfigureCacheKey(entity.Id);

            var cachedEntity = ConfigureCacheCargo(entity);

            SetCachedEntity(key, cachedEntity);
        }

		private void CheckIfCachedValueExists(RedisValue value, ref CachedCargo result, string key)
		{
			if (!value.HasValue) return;

			result = JsonConvert.DeserializeObject<CachedCargo>(value);

			result.AccessCount++;

			result.LastAccessed = DateTime.UtcNow;

			UpdateCachedEntity(key, result);
		}

		public CachedCargo GetById(int id)
        {
            CachedCargo result = null;

            var key = ConfigureCacheKey(id);

            var cachedValue = _database.StringGet(key);

	        CheckIfCachedValueExists(cachedValue, ref result, key);

			return result;
        }	   

		public IEnumerable<CachedCargo> PopAllCreated()
        {
	        if (!_settings.UseWriteBehindStrategy) throw new ServiceException(ErrorMessage);

	        var result = new List<CachedCargo>();

	        var cachedValues = _database.SetScan("Add_" + KeyHeader).ToList();

	        while (cachedValues.Any())
	        {
		        result.AddRange(cachedValues
			        .Select(value => JsonConvert.DeserializeObject<CachedCargo>(value)));

		        _database.SetRemove("Add_" + KeyHeader, cachedValues.ToArray());

		        cachedValues = _database.SetScan("Add_" + KeyHeader).ToList();
	        }

	        return result;
        }

        public void SetAsCreated(Cargo entity)
        {
	        if (_settings.UseWriteBehindStrategy)
	        {
		        var key = "Add_" + typeof(Cargo).Name;

		        var cachedEntity = ConfigureCacheCargo(entity);

		        var value = JsonConvert.SerializeObject(cachedEntity);

		        _database.SetAdd(key, value);
	        }
	        else
	        {
		        throw new ServiceException(ErrorMessage);
	        }
        }

        public IEnumerable<CachedCargo> GetAll()
        {
	        var keys = _server.Keys(pattern: typeof(Cargo).Name + "_*");

            var cachedValues = _database.StringGet(keys.ToArray());

	        return
		        (from value in cachedValues where value.HasValue select JsonConvert.DeserializeObject<CachedCargo>(value))
		        .ToList();
        }

        private static CachedCargo ConfigureCacheCargo(Cargo entity)
        {
            var cachedEntity = new CachedCargo
            {
                Entity = entity,
                AccessCount = 1,
                CacheUniqueKey = Guid.NewGuid()
            };

            return cachedEntity;
        }

        private void SetCachedEntity(string key, CachedCargo cachedCargo)
        {
            _database.StringSet(
                key,
                JsonConvert.SerializeObject(cachedCargo),
                TimeSpan.FromMinutes(_settings.ExpirationMinutes));
        }

        private void UpdateCachedEntity(string key, CachedCargo cachedCargo)
        {
            var expiry = _database.StringGetWithExpiry(key).Expiry;

            _database.StringSet(key, JsonConvert.SerializeObject(cachedCargo), expiry);
        }

        private static string ConfigureCacheKey(int id)
        {
            var key = KeyHeader + "_" + id;

            return key;
        }
    }
}