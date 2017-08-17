using System;
using System.Web.Http;
using AutoMapper;
using Cache.DAL.Entities;
using Cache.DAL.Interfaces;
using Cache.WEB.Models;

namespace Cache.WEB.Controllers
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

            _repository.Create(cargo);

            return Ok();
        }

		private static int GetBetweenIds(int min, int max)
		{
			return new Random().Next(min, max + 1);
		}
	}
}