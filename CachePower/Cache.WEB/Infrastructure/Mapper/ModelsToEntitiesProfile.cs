using AutoMapper;
using Cache.DAL.Entities;
using Cache.WEB.Models;

namespace Cache.WEB.Infrastructure.Mapper
{
    public class ModelsToEntitiesProfile : Profile
    {
        public ModelsToEntitiesProfile()
        {
            CreateMap<CargoModel, Cargo>();
        }
    }
}