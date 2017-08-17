using System;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;
using Caghing.Dal.Entities;

namespace CachePower.WEB.Controllers
{
    [RoutePrefix("api/cargoes")]
    public class CargoController : ApiController
    {
        private readonly IRepository<Cargo> _repository;
        private readonly IMapper _mapper;

        public CargoController(IRepository<Cargo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var cargo = _repository.Get(id);

            if (cargo == null)
            {
                return NotFound();
            }

            var cargoApiModel = _mapper.Map<CargoApiModel>(cargo);

            return Ok(cargoApiModel);
        }

        [HttpGet]
        [Route("random")]
        public IHttpActionResult Get()
        {
            var id = GetRandomId(500, 700);
            var cargo = _repository.Get(id);

            if (cargo == null)
            {
                return NotFound();
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

            _repository.Create(cargo);

            return Ok();
        }

        private int GetRandomId(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }
    }
}