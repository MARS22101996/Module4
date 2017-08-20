using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cache.DAL.Entities;

namespace Cache.DAL.Repositories.Interfaces
{
    public interface ICacheCargoRepository
    {
	    Task<IEnumerable<object>> GetAsync(Func<CachedCargo, object> orderPredicate, Func<CachedCargo, object> selectPredicate,
		    int take);

		Task ConfigureAsync(Cargo item);

		Task CreateInTheCacheAsync(Cargo entity);

		Task<IEnumerable<CachedCargo>> GetAllAsync();

		Task<CachedCargo> GetByIdAsync(int id);

        Task<IEnumerable<CachedCargo>> PopAllCreatedAsync(); 
    }
}