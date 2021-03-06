﻿using System;
using System.Web.Http;
using AutoMapper;
using Cache.DAL.Entities;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Models;

namespace Cache.WEB.Controllers
{
    [RoutePrefix("api/caching/cargoes")]
    public class CachedController : ApiController
    {
        private readonly IRepository _repository;
        private readonly ICacheCargoRepository _cacheCargoRepository;
        private readonly IMapper _mapper;
	    private readonly Random _random;
	    private const int MinValue = 500;
		private const int MaxValue = 700;

		public CachedController(
            IRepository repository,
            ICacheCargoRepository cacheCargoRepository,
            IMapper mapper)
        {
            _repository = repository;
            _cacheCargoRepository = cacheCargoRepository;
            _mapper = mapper;
	        _random = new Random();

        }

		private int GetBetweenIds(int min, int max)
		{
			var randomValue = _random.Next(min, max + 1);

			return randomValue;
		}

		[HttpGet]
        [Route("randomid")]
        public IHttpActionResult Get()
        {
            var id = GetBetweenIds(MinValue, MaxValue);

            var cargoCached = _cacheCargoRepository.GetById(id);

	        Cargo cargo = null;

	        if (cargoCached != null)
	        {
		        cargo = cargoCached.EntityCargo;
	        }
			
            if (cargo == null)
            {
                cargo = _repository.GetById(id);

                if (cargo == null)
                {
                    return NotFound();
                }

                _cacheCargoRepository.Configure(cargo);
            }

            var cargoApiModel = _mapper.Map<CargoModel>(cargo);

            return Ok(cargoApiModel);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(CargoModel cargoModel)
        {
	        if (!ModelState.IsValid) return BadRequest(ModelState);

	        var cargo = _mapper.Map<Cargo>(cargoModel);

	        _cacheCargoRepository.CreateInTheCache(cargo);

	        return Ok();
        }

	}
}