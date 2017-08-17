using System.Web.Http;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB.Schedulers.Jobs
{
    public class SaveCreatedCacheJob : IJob
    {
        private readonly ICacheRepository _cacheRepository = (ICacheRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheRepository));
        private readonly IRepository _repository = (IRepository)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(IRepository));

        public string JobName => "Save_To_Database";

        public void Execute()
        {
            var createdEntities = _cacheRepository.PopAllCreated();

            foreach (var cachedEntity in createdEntities)
            {
                _repository.Create(cachedEntity.Entity);
            }
        }
    }
}