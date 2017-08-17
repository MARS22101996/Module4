using AutoMapper;
using CachePower.DAL.Entities;
using CachePower.WEB.Models;

namespace CachePower.WEB.MapperProfiles
{
    public class EntitiesToApiModelsProfile : Profile
    {
        public EntitiesToApiModelsProfile()
        {
            CreateMap<Cargo, CargoModel>();
        }
    }
}