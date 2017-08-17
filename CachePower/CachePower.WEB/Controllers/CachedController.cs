using System;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;

namespace CachePower.WEB.Controllers
{
    [RoutePrefix("api/caching/cargoes")]
    public class CachedController : ApiController
    {
        private readonly IRepository _repository;
        private readonly ICacheCargoRepository _cacheCargoRepository;
        private readonly IMapper _mapper;

        public CachedController(
            IRepository repository,
            ICacheCargoRepository cacheCargoRepository,
            IMapper mapper)
        {
            _repository = repository;
            _cacheCargoRepository = cacheCargoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("randomid")]
        public IHttpActionResult Get()
        {
            var id = GetBetweenIds(500, 700);

            var cargoCached = _cacheCargoRepository.Get(id);

	        Cargo cargo = null;

	        if (cargoCached != null)
	        {
		        cargo = cargoCached.Entity;
	        }
			
            if (cargo == null)
            {
                cargo = _repository.GetById(id);

                if (cargo == null)
                {
                    return NotFound();
                }

                _cacheCargoRepository.Set(cargo);
            }

            var cargoApiModel = _mapper.Map<CargoApiModel>(cargo);

            return Ok(cargoApiModel);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(CargoApiModel cargoApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cargo = _mapper.Map<Cargo>(cargoApiModel);

            _cacheCargoRepository.SetAsCreated(cargo);

            return Ok();
        }

		private static int GetBetweenIds(int min, int max)
		{
			return new Random().Next(min, max + 1);
		}
	}
}