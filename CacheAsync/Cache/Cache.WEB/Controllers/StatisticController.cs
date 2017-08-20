using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Cache.DAL.Entities;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Models;
using Cache.WEB.Settings;
using StackExchange.Redis;

namespace Cache.WEB.Controllers
{
    [RoutePrefix("api/cargostatistic")]
    public class StatisticController : ApiController
    {
        private readonly ICacheCargoRepository _cacheCargoRepository;
        private readonly IServer _redisServer;
        private readonly IMapper _mapper;

        public StatisticController(IServer redisServer, ICacheCargoRepository cacheCargoRepository, IMapper mapper)
        {
            _redisServer = redisServer;
            _cacheCargoRepository = cacheCargoRepository;
            _mapper = mapper;
        }

		[HttpGet]
		[Route("gettop/{count}")]
		public async Task<IHttpActionResult> GetTopNcargoes(int count)
		{
			Func<CachedCargo, object> orderPredicate = cargo => cargo.LastAccessed;

			Func<CachedCargo, object> selectPredicate = cargo => cargo.EntityCargo;

			var cargoes = await _cacheCargoRepository.GetAsync(orderPredicate, selectPredicate, count);

			var cargoApiModels = _mapper.Map<IEnumerable<CargoModel>>(cargoes);

			return Ok(cargoApiModels);
		}

		[HttpGet]
        [Route("ratio")]
        public async Task<IHttpActionResult> GetRatio()
        {
            var infomatinRedis = await _redisServer.InfoAsync();

            var statistic = infomatinRedis.First(element => element.Key.Equals(StatisticSettings.Stats));

            var cahceRatio = new RatioModel
            {
                Hits = GetHitsOrMisses(statistic, StatisticSettings.Hits),

                Misses = GetHitsOrMisses(statistic, StatisticSettings.Misses)
			};

            return Ok(cahceRatio);
        }

	    private long GetHitsOrMisses(IGrouping<string, KeyValuePair<string, string>> statistic, string parameter)
	    {
		    if (statistic == null) throw new ArgumentNullException(nameof(statistic));

		    var stat = statistic.First(element => element.Key.Equals(parameter)).Value;

		    return Convert.ToInt64(stat);
	    }
    }
}