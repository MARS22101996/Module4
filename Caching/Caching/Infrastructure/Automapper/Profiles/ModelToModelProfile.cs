using AutoMapper;
using Caching.Models;
using Caghing.Dal.Entities;

namespace Caching.Infrastructure.Automapper.Profiles
{
    public class ModelToModelProfile : Profile
    {
        public ModelToModelProfile()
        {
            CreateMap<CargoModel, Cargo>();
        }
    }
}
