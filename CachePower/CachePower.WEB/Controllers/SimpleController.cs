using System;
using System.Web.Http;
using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Models;

namespace CachePower.WEB.Controllers
{
    [RoutePrefix("api/cargoes")]
    public class SimpleController : ApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public SimpleController(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("randomid")]
        public IHttpActionResult Get()
        {
            var id = GetBetweenIds(500, 700);
            var cargo = _repository.GetById(id);

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

		private static int GetBetweenIds(int min, int max)
		{
			return new Random().Next(min, max + 1);
		}
	}
}