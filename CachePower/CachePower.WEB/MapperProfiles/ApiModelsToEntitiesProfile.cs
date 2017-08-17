using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.WEB.Models;
using Caghing.Dal.Entities;

namespace CachePower.WEB.MapperProfiles
{
    public class ApiModelsToEntitiesProfile : Profile
    {
        public ApiModelsToEntitiesProfile()
        {
            CreateMap<CargoApiModel, Cargo>();
        }
    }
}