using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;
using Caghing.Dal.Entities;
using StackExchange.Redis;

namespace CachePower.WEB.Controllers
{
    [RoutePrefix("api/statistic")]
    public class StatisticController : ApiController
    {
        private readonly ICacheRepository<Cargo> _cacheRepository;
        private readonly IServer _redisServer;
        private readonly IMapper _mapper;

        public StatisticController(IServer redisServer, ICacheRepository<Cargo> cacheRepository, IMapper mapper)
        {
            _redisServer = redisServer;
            _cacheRepository = cacheRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("ratio")]
        public IHttpActionResult Ratio()
        {
            var info = _redisServer.Info();
            var stats = info.First(element => element.Key.Equals("Stats"));

            var cahceRatio = new CacheRatioApiModel
            {
                Hits = Convert.ToInt64(stats.First(element => element.Key.Equals("keyspace_hits")).Value),
                Misses = Convert.ToInt64(stats.First(element => element.Key.Equals("keyspace_misses")).Value)
            };

            return Ok(cahceRatio);
        }

        [HttpGet]
        [Route("getlast/{number}")]
        public IHttpActionResult GetLast(int number)
        {
            var cachedCargoes = _cacheRepository.GetAll();
            var cargoes = cachedCargoes
                .OrderByDescending(cargo => cargo.LastAccessed)
                .Select(cargo => cargo.Entity)
                .Take(number);

            var cargoApiModels = _mapper.Map<IEnumerable<CargoApiModel>>(cargoes);

            return Ok(cargoApiModels);
        }
    }
}