using System;
using System.Web.Http;
using AutoMapper;
using Cache.DAL.Entities;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Models;

namespace Cache.WEB.Controllers
{
    [RoutePrefix("api/cargoes")]
    public class SimpleController : ApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
		private readonly Random _random;


		public SimpleController(IRepository repository, IMapper mapper)
        {
            _repository = repository;
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
	}
}