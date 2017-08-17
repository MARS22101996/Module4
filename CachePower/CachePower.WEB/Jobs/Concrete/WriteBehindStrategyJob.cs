using System.Web.Http;
using Cache.DAL.Interfaces;
using Cache.WEB.Interfaces;
using Cache.DAL.Entities;

namespace Cache.WEB.Schedulers.Jobs
{
    public class WriteBehindStrategyJob : IJob
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly IRepository _repository = (IRepository)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(IRepository));

        public string Name => "WriteBehindStrategy_Job";

        public void Run()
        {
            var createdEntities = _cacheCargoRepository.PopAllCreated();

            foreach (var cachedEntity in createdEntities)
            {
                _repository.Create(cachedEntity.Entity);
            }
        }
    }
}