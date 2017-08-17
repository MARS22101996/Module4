using System.Web.Http;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB.Schedulers.Jobs
{
    public class SaveCreatedCacheJob<TEntity> : IJob where TEntity : BaseType, new()
    {
        private readonly ICacheRepository<TEntity> _cacheRepository = (ICacheRepository<TEntity>)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheRepository<TEntity>));
        private readonly IRepository<TEntity> _repository = (IRepository<TEntity>)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(IRepository<TEntity>));

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