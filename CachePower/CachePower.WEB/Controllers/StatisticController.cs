using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;
using StackExchange.Redis;

namespace CachePower.WEB.Controllers
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
            var cachedCargoes = _cacheCargoRepository.GetAll();
            var cargoes = cachedCargoes
                .OrderByDescending(cargo => cargo.LastAccessed)
                .Select(cargo => cargo.Entity)
                .Take(number);

            var cargoApiModels = _mapper.Map<IEnumerable<CargoApiModel>>(cargoes);

            return Ok(cargoApiModels);
        }
    }
}