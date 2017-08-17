using AutoMapper;
using Cache.DAL.Entities;
using Cache.WEB.Models;

namespace Cache.WEB.MapperProfiles
{
    public class ApiModelsToEntitiesProfile : Profile
    {
        public ApiModelsToEntitiesProfile()
        {
            CreateMap<CargoModel, Cargo>();
        }
    }
}