using System;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;

namespace CachePower.WEB.Controllers
{
    [RoutePrefix("api/usecache/cargoes")]
    public class CargoCachedController : ApiController
    {
        private readonly IRepository _repository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;

        public CargoCachedController(
            IRepository repository,
            ICacheRepository cacheRepository,
            IMapper mapper)
        {
            _repository = repository;
            _cacheRepository = cacheRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("random")]
        public IHttpActionResult Get()
        {
            var id = GetRandomId(500, 700);
            var cargo = _cacheRepository.Get(id)?.Entity;

            if (cargo == null)
            {
                cargo = _repository.Get(id);

                if (cargo == null)
                {
                    return NotFound();
                }

                _cacheRepository.Set(cargo);
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

            _cacheRepository.SetAsCreated(cargo);

            return Ok();
        }

        private int GetRandomId(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }
    }
}