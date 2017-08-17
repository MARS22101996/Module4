using System.Web.Http;
using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB.Schedulers.Jobs
{
    public class UpdateCacheExpirationJob<TEntity> : IJob where TEntity : BaseType, new()
    {
        private readonly ICacheRepository<TEntity> _cacheRepository = (ICacheRepository<TEntity>)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheRepository<TEntity>));
        private readonly ICacheSettings _settings = (ICacheSettings)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(ICacheSettings));

        public string JobName => "Update_Expirations";

        public void Execute()
        {
            var cargoes = _cacheRepository.GetAll();

            foreach (var cargo in cargoes)
            {
                if (cargo.AccessCount >= _settings.AccessCountEnoughForUpdateExpiration)
                {
                    _cacheRepository.Set(cargo.Entity);
                }
            }
        }
    }
}