using AutoMapper;
using Cache.DAL.Entities;
using Cache.WEB.Models;

namespace Cache.WEB.MapperProfiles
{
    public class EntitiesToApiModelsProfile : Profile
    {
        public EntitiesToApiModelsProfile()
        {
            CreateMap<Cargo, CargoModel>();
        }
    }
}