using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using CachePower.DAL;
using CachePower.DAL.Context;
using CachePower.DAL.Interfaces;
using CachePower.DAL.Repositories;
using CachePower.WEB.Interfaces;
using CachePower.WEB.MapperProfiles;
using CachePower.WEB.Schedulers;
using CachePower.WEB.Schedulers.Jobs;
using CachePower.WEB.Settings;
using StackExchange.Redis;

namespace CachePower.WEB
{
    public class ConfigDI
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

	        builder.RegisterInstance(SimpleConfig.Configuration.Load<CacheSettings>()).As<ICacheSettings>();

	        builder.RegisterType<WriteBehindStrategyJob>().As<IJob>().InstancePerLifetimeScope();

	        builder.RegisterType<RefreshAheadStrategyJob>().As<IJob>().InstancePerLifetimeScope();

	        builder.RegisterType<Scheduler>().As<IScheduler>().InstancePerLifetimeScope();

	        builder.RegisterType<SchedulerCreater>().As<ISchedulerConfigurer>().InstancePerLifetimeScope();

	        var connection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings.Get("redisConnection"));

	        var endpoint = connection.GetEndPoints().First();

	        var server = connection.GetServer(endpoint);

	        builder.RegisterInstance(connection.GetDatabase()).As<IDatabase>();

	        builder.RegisterInstance(server).As<IServer>();

	        builder.RegisterType<ShipmentContext>().As<ShipmentContext>().InstancePerLifetimeScope();

	        builder.RegisterType<CargoRepository>().As<IRepository>().InstancePerLifetimeScope();

	        builder.RegisterType<CacheCargoRepository>().As<ICacheCargoRepository>().InstancePerLifetimeScope();

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