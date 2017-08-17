using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.WEB.Models;
using Caghing.Dal.Entities;

namespace CachePower.WEB.MapperProfiles
{
    public class EntitiesToApiModelsProfile : Profile
    {
        public EntitiesToApiModelsProfile()
        {
            CreateMap<Cargo, CargoApiModel>();
        }
    }
}