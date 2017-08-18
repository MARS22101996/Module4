using System;
using System.Collections.Generic;
using System.Linq;
using Cache.DAL.Entities;
using Cache.DAL.Exceptions;
using Cache.DAL.Repositories.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Cache.DAL.Repositories.Concrete
{
    public class CacheCargoRepository : ICacheCargoRepository
    {
        private readonly IServer _server;
        private readonly IDatabase _database;
        private readonly ICacheSettings _settings;
	    private const string KeyHeader = "Cargo";
	    private const string ErrorMessage = "This action suits another strategy";
	    private const string ActionKey = "Add_";


		public CacheCargoRepository(IServer server, IDatabase database, ICacheSettings settings)
        {
            _server = server;
            _database = database;
            _settings = settings;
        }

		public  IEnumerable<object> Get(Func<CachedCargo, object> orderPredicate, Func<CachedCargo, object> selectPredicate,
		 int take)
		{
			var cargo = GetAll();

			var cargoes = cargo
				.OrderByDescending(orderPredicate)
				.Select(selectPredicate)
				.Take(take);

			return cargoes;
		}

		
		public IEnumerable<CachedCargo> PopAllCreated()
        {
	        if (!_settings.UseWriteBehindStrategy) throw new ServiceException(ErrorMessage);

	        var result = new List<CachedCargo>();

	        var cachedValues = _database.SetScan(ActionKey + KeyHeader).ToList();

	        while (cachedValues.Any())
	        {
		        result.AddRange(cachedValues
			        .Select(value => JsonConvert.DeserializeObject<CachedCargo>(value)));

		        _database.SetRemove(ActionKey + KeyHeader, cachedValues.ToArray());

		        cachedValues = _database.SetScan(ActionKey + KeyHeader).ToList();
	        }

	        return result;
        }

        public void CreateInTheCache(Cargo entity)
        {
	        if (_settings.UseWriteBehindStrategy)
	        {
		        var key = ActionKey + typeof(Cargo).Name;

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
	        var keys = _server.Keys(pattern: KeyHeader + "_*");

            var cachedValues = _database.StringGet(keys.ToArray());

	        return
		        (from value in cachedValues where value.HasValue select JsonConvert.DeserializeObject<CachedCargo>(value))
		        .ToList();
        }

		public void Configure(Cargo item)
		{
			var key = ConfigureCacheKey(item.Id);

			var cachedEntity = ConfigureCacheCargo(item);

			InitCachedCargo(key, cachedEntity);
		}

		private void CheckIfCachedValueExists(RedisValue value, ref CachedCargo result, string key)
		{
			if (!value.HasValue) return;

			result = JsonConvert.DeserializeObject<CachedCargo>(value);

			result.AccessCount++;

			result.LastAccessed = DateTime.UtcNow;

			UpdateCargoInTheCache(key, result);
		}

		public CachedCargo GetById(int id)
		{
			CachedCargo result = null;

			var key = ConfigureCacheKey(id);

			var cachedValue = _database.StringGet(key);

			CheckIfCachedValueExists(cachedValue, ref result, key);

			return result;
		}

		private static CachedCargo ConfigureCacheCargo(Cargo entity)
        {
            var cachedEntity = new CachedCargo
            {
                EntityCargo = entity,
                AccessCount = 1,
                Key = Guid.NewGuid()
            };

            return cachedEntity;
        }

        private void InitCachedCargo(string key, CachedCargo cachedCargo)
        {
	        var serializedObject = JsonConvert.SerializeObject(cachedCargo);

	        var time = TimeSpan.FromMinutes(_settings.ExpirationMinutes);

			_database.StringSet(
                key,
				serializedObject,
				time);
        }

        private void UpdateCargoInTheCache(string key, CachedCargo cachedCargo)
        {
            var expiry = _database.StringGetWithExpiry(key).Expiry;

	        var serializedCargo = JsonConvert.SerializeObject(cachedCargo);

			_database.StringSet(key, serializedCargo, expiry);
        }

        private static string ConfigureCacheKey(int id)
        {
            var key = KeyHeader + "_" + id;

            return key;
        }
    }
}