using System.Web.Http;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB.Jobs.Concrete
{
    public class RefreshAheadStrategyJob : IStrategyJob 
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly ICacheSettings _settings = (ICacheSettings)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(ICacheSettings));

        public void Run()
        {
            var cargoes = _cacheCargoRepository.GetAll();

            foreach (var cargo in cargoes)
            {
                if (cargo.AccessCount >= _settings.AccessCountLimit)
                {
                    _cacheCargoRepository.Configure(cargo.EntityCargo);
                }
            }
        }
    }
}