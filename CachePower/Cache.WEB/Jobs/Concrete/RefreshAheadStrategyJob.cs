using System.Web.Http;
using Cache.DAL.Enums;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB.Jobs.Concrete
{
    public class RefreshAheadStrategyJob : IJob 
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly ICacheSettings _settings = (ICacheSettings)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(ICacheSettings));

        public JobType Name => JobType.RefreshAhead;

        public void Run()
        {
            var cargoes = _cacheCargoRepository.GetAll();

            foreach (var cargo in cargoes)
            {
                if (cargo.AccessCount >= _settings.AccessCountEnoughForUpdateExpiration)
                {
                    _cacheCargoRepository.Configure(cargo.EntityCargo);
                }
            }
        }
    }
}