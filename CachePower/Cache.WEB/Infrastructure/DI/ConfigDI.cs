using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using Cache.DAL.Context;
using Cache.DAL.Repositories;
using Cache.DAL.Repositories.Concrete;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Infrastructure.Mapper;
using Cache.WEB.Jobs;
using Cache.WEB.Jobs.Concrete;
using Cache.WEB.Jobs.Interfaces;
using Cache.WEB.Settings;
using StackExchange.Redis;

namespace Cache.WEB.Infrastructure.DI
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
			builder.RegisterType<ShipmentContext>().As<ShipmentContext>().InstancePerLifetimeScope();

			builder.RegisterType<CargoRepository>().As<IRepository>().InstancePerLifetimeScope();

			builder.RegisterType<CacheCargoRepository>().As<ICacheCargoRepository>().InstancePerLifetimeScope();

			builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile));

			builder.RegisterType<ModelsToEntitiesProfile>().As<Profile>();

			builder.RegisterType<EntitiesToModelsProfile>().As<Profile>();

			builder.Register(c => new MapperConfiguration(cfg =>
			{
				foreach (var profile in c.Resolve<IEnumerable<Profile>>())
				{
					cfg.AddProfile(profile);
				}
			})).AsSelf().SingleInstance();

			builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>()
				.InstancePerLifetimeScope();

			builder.RegisterApiControllers(currentAssembly);

	        builder.RegisterInstance(SimpleConfig.Configuration.Load<CacheSettings>()).As<ICacheSettings>();

	        builder.RegisterType<WriteBehindStrategyJob>().As<IStrategyJob>().InstancePerLifetimeScope();

	        builder.RegisterType<RefreshAheadStrategyJob>().As<IStrategyJob>().InstancePerLifetimeScope();

	        builder.RegisterType<JobScheduler>().As<IJobScheduler>().InstancePerLifetimeScope();

	        builder.RegisterType<SchedulerCreater>().As<ISchedulerConfigurer>().InstancePerLifetimeScope();

	        var connection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings.Get("redisConnection"));

	        var endpoint = connection.GetEndPoints().First();

	        var server = connection.GetServer(endpoint);

	        builder.RegisterInstance(connection.GetDatabase()).As<IDatabase>();

	        builder.RegisterInstance(server).As<IServer>();
        }
    }
}