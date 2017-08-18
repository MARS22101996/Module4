using System;
using System.Web.Http;
using AutoMapper;
using Cache.DAL.Entities;
using Cache.DAL.Interfaces;
using Cache.WEB.Models;

namespace Cache.WEB.Controllers
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

            var cargoCached = _cacheCargoRepository.GetById(id);

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

                _cacheCargoRepository.Configure(cargo);
            }

            var cargoApiModel = _mapper.Map<CargoModel>(cargo);

            return Ok(cargoApiModel);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(CargoModel cargoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cargo = _mapper.Map<Cargo>(cargoModel);

            _cacheCargoRepository.SetAsCreated(cargo);

            return Ok();
        }

		private static int GetBetweenIds(int min, int max)
		{
			return new Random().Next(min, max + 1);
		}
	}
}