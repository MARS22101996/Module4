using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB.Jobs.Concrete
{
    public class WriteBehindStrategyJob : IStrategyJob
    {
        private readonly ICacheCargoRepository _cacheCargoRepository = (ICacheCargoRepository)GlobalConfiguration
            .Configuration.DependencyResolver.GetService(typeof(ICacheCargoRepository));

        private readonly IRepository _repository = (IRepository)GlobalConfiguration.Configuration
            .DependencyResolver.GetService(typeof(IRepository));

        public async Task Run()
        {
            var createdEntities = await _cacheCargoRepository.PopAllCreatedAsync();

			var operations = new List<Task>();

            foreach (var cachedEntity in createdEntities)
            {
				operations.Add(_repository.CreateAsync(cachedEntity.EntityCargo));
            }

			await Task.WhenAll(operations);
		}
    }
}