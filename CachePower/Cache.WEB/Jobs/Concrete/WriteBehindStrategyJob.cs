using System.Web.Http;
using Cache.DAL.Enums;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB.Jobs.Concrete
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
                _repository.Create(cachedEntity.EntityCargo);
            }
        }
    }
}