using AutoMapper;
using Caching.Infrastructure.Automapper.Profiles;

namespace Caching.Infrastructure.Automapper
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new ModelToModelProfile());
                cfg.AddProfile(new EntityToDtoProfile());
            });
        }
    }
}
