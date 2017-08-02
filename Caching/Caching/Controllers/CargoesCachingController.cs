using System;
using System.Web.Http;
using AutoMapper;
using Caching.Models;
using Caghing.Dal.Entities;
using Caghing.Dal.Interfaces;

namespace Caching.Controllers
{
    [RoutePrefix("api/cache/cargoes")]
    public class CargoesCachingController : ApiController
    {
        private readonly ICargoCachedRepository _repositoryCargo;

        public CargoesCachingController(ICargoCachedRepository repository)
        {
            _repositoryCargo = repository;
        }

        [HttpGet]
        [Route("randomid")]
        public IHttpActionResult Get()
        {
            var id = GetBetweenIds(500, 700);

            var cargo = _repositoryCargo.GetById(id);

            if (cargo == null)
            {
                return NotFound();
            }

            var cargoApiModel = Mapper.Map<CargoModel>(cargo);

            return Ok(cargoApiModel);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(CargoModel cargoApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cargo = Mapper.Map<Cargo>(cargoApiModel);

            _repositoryCargo.Create(cargo);

            return Ok();
        }

        private static int GetBetweenIds(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }
    }
}
