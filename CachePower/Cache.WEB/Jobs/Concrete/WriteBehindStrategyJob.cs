using System.Web.Http;
using Cache.DAL.Interfaces;
using Cache.WEB.Interfaces;
using Cache.DAL.Entities;
using Cache.DAL.Enums;

namespace Cache.WEB.Schedulers.Jobs
{
    public class WriteBehindStrategyJob : IJob
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly IRepository _repository = (IRepository)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(IRepository));

        public JobType Name => JobType.WriteBehind;

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