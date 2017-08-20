using AutoMapper;
using Cache.DAL.Entities;
using Cache.WEB.Models;

namespace Cache.WEB.Infrastructure.Mapper
{
    public class EntitiesToModelsProfile : Profile
    {
        public EntitiesToModelsProfile()
        {
            CreateMap<Cargo, CargoModel>();
        }
    }
}