using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using CachePower.DAL;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.DAL.Repositories;
using CachePower.WEB.Configuration;
using CachePower.WEB.Interfaces;
using CachePower.WEB.MapperProfiles;
using CachePower.WEB.Schedulers;
using CachePower.WEB.Schedulers.Jobs;
using Caghing.Dal.Entities;
using StackExchange.Redis;

namespace CachePower.WEB
{
    public class AutofacConfig
    {
        public static IContainer Setup()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();

            RegisterDependencies(currentAssembly, builder);

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            return container;
        }

        private static void RegisterDependencies(Assembly currentAssembly, ContainerBuilder builder)
        {
            builder.RegisterApiControllers(currentAssembly);

            RegisterRedisCache(builder);
            RegisterMapper(builder);
            RegisterRepositories(builder);
            RegisterScheduler(builder);
            RegisterJobs(builder);
            RegisterConfiguration(builder);
        }

        private static void RegisterConfiguration(ContainerBuilder builder)
        {
            builder.RegisterInstance(SimpleConfig.Configuration.Load<CacheSettings>()).As<ICacheSettings>();
        }

        private static void RegisterJobs(ContainerBuilder builder)
        {
            builder.RegisterType<SaveCreatedCacheJob<Cargo>>().As<IJob>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateCacheExpirationJob<Cargo>>().As<IJob>().InstancePerLifetimeScope();
        }

        private static void RegisterScheduler(ContainerBuilder builder)
        {
            builder.RegisterType<HangfireScheduler>().As<IScheduler>().InstancePerLifetimeScope();
            builder.RegisterType<SchedulerInitializer>().As<ISchedulerInitializer>().InstancePerLifetimeScope();
        }

        private static void RegisterRedisCache(ContainerBuilder builder)
        {
            var connection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings.Get("redisConnection"));
            var endpoint = connection.GetEndPoints().First();
            var server = connection.GetServer(endpoint);

            builder.RegisterInstance(connection.GetDatabase()).As<IDatabase>();
            builder.RegisterInstance(server).As<IServer>();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<ShipmentDbContext>().As<ShipmentDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<CargoRepository>().As<IRepository<Cargo>>().InstancePerLifetimeScope();
            builder.RegisterType<CacheRepository<Cargo>>().As<ICacheRepository<Cargo>>().InstancePerLifetimeScope();
        }

        private static void RegisterMapper(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile));

            builder.RegisterType<ApiModelsToEntitiesProfile>().As<Profile>();
            builder.RegisterType<EntitiesToApiModelsProfile>().As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}