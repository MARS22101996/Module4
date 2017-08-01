using AutoMapper;
using Caching.Models;
using Caghing.Dal.Entities;

namespace Caching.Infrastructure.Automapper.Profiles
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile()
        {
            CreateMap<Cargo, CargoModel>();
        }
    }
}
