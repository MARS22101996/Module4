using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
	    private const string ActionKey = "AddCargo_";


		public CacheCargoRepository(IServer server, IDatabase database, ICacheSettings settings)
        {
            _server = server;
            _database = database;
            _settings = settings;
        }

	    public async Task<IEnumerable<object>> GetAsync(Func<CachedCargo, 
			object> orderPredicate, 
			Func<CachedCargo, object> selectPredicate,
		    int take)
	    {
		    var cargo = await GetAllAsync();

		    var cargoes = cargo
			    .OrderByDescending(orderPredicate)
			    .Select(selectPredicate)
			    .Take(take);

		    return cargoes;
	    }


		public async Task<IEnumerable<CachedCargo>> PopAllCreatedAsync()
        {
	        if (!_settings.IsUseWriteBehindStrategy) throw new ServiceException(ErrorMessage);

	        var result = new List<CachedCargo>();

	        var cachedValues = _database.SetScan(ActionKey + KeyHeader, "a*").ToList();

	        while (cachedValues.Any())
	        {
		        result.AddRange(cachedValues
			        .Select(value => JsonConvert.DeserializeObject<CachedCargo>(value)));

		        await _database.SetRemoveAsync(ActionKey + KeyHeader, cachedValues.ToArray());

		        cachedValues = _database.SetScan(ActionKey + KeyHeader, "a*").ToList();
	        }

	        return result;
        }

        public Task CreateInTheCacheAsync(Cargo entity)
        {
	        if (!_settings.IsUseWriteBehindStrategy) throw new ServiceException(ErrorMessage);

			const string key = ActionKey + KeyHeader;

	        var cachedEntity = ConfigureCacheCargo(entity);

	        var value = JsonConvert.SerializeObject(cachedEntity);

	        return _database.SetAddAsync(key, value);
        }

		public async Task<IEnumerable<CachedCargo>> GetAllAsync()
        {
	        var keys = _server.Keys(pattern: KeyHeader + "_*");

            var cachedValues = await _database.StringGetAsync(keys.ToArray());

	        return
		        (from value in cachedValues where value.HasValue select JsonConvert.DeserializeObject<CachedCargo>(value))
		        .ToList();
		}

		public Task ConfigureAsync(Cargo item)
		{
			var key = ConfigureCacheKey(item.Id);

			var cachedEntity = ConfigureCacheCargo(item);

			return InitCachedCargoAsync(key, cachedEntity);
		}

		public async Task<CachedCargo> GetByIdAsync(int id)
		{
			var key = ConfigureCacheKey(id);

			var cachedValue = await _database.StringGetAsync(key);

			if (!cachedValue.HasValue) return null;

			var result = JsonConvert.DeserializeObject<CachedCargo>(cachedValue);

			result.AccessCount++;

			result.LastAccessed = DateTime.UtcNow;

			await UpdateCargoInTheCacheAsync(key, result);

			return result;
		}

		private  CachedCargo ConfigureCacheCargo(Cargo entity)
        {
            var cachedEntity = new CachedCargo
            {
                EntityCargo = entity,
                AccessCount = 1,
                Key = Guid.NewGuid()
            };

            return cachedEntity;
        }

        private Task InitCachedCargoAsync(string key, CachedCargo cachedCargo)
        {
	        var serializedObject = JsonConvert.SerializeObject(cachedCargo);

	        var time = TimeSpan.FromMinutes(_settings.ExpirationInterval);

			return _database.StringSetAsync(
                key,
				serializedObject,
				time);
        }

        private Task UpdateCargoInTheCacheAsync(string key, CachedCargo cachedCargo)
        {
            var expiry = _database.StringGetWithExpiry(key).Expiry;

	        var serializedCargo = JsonConvert.SerializeObject(cachedCargo);

			var task = _database.StringSetAsync(key, serializedCargo, expiry);

			return task;
        }

        private static string ConfigureCacheKey(int id)
        {
            var key = KeyHeader + "_" + id;

            return key;
        }
    }
}