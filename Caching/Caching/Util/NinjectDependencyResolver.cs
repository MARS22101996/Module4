using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Ninject;
using Caghing.Dal.Interfaces;
using Caghing.Dal.Repositories;
using StackExchange.Redis;

namespace Caching.Util
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            _kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            _kernel.Bind<ICargoRepository>().To<CargoRepository>();
            _kernel.Bind<ICargoCachedRepository>().To<CargoCachedRepository>();
            var connection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings.Get("redisConnection"));
            _kernel.Bind<IDatabase>().ToMethod(p => connection.GetDatabase());
        }
    }
}