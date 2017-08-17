using System.Web.Http;
using Cache.DAL.Interfaces;
using Cache.WEB.Interfaces;
using Cache.DAL.Entities;

namespace Cache.WEB.Schedulers.Jobs
{
    public class RefreshAheadStrategyJob : IJob 
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly ICacheSettings _settings = (ICacheSettings)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(ICacheSettings));

        public string Name => "RefreshAheadStrategy";

        public void Run()
        {
            var cargoes = _cacheCargoRepository.GetAll();

            foreach (var cargo in cargoes)
            {
                if (cargo.AccessCount >= _settings.AccessCountEnoughForUpdateExpiration)
                {
                    _cacheCargoRepository.Configure(cargo.Entity);
                }
            }
        }
    }
}