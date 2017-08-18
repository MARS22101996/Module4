using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cache.DAL.Entities;

namespace Cache.DAL.Interfaces
{
    public interface ICacheCargoRepository
    {
	    IEnumerable<object> Get(Func<CachedCargo, object> orderPredicate, Func<CachedCargo, object> selectPredicate,
		    int take);

		void Configure(Cargo item);

		void CreateInTheCache(Cargo entity);

		IEnumerable<CachedCargo> GetAll();

		CachedCargo GetById(int id);

        IEnumerable<CachedCargo> PopAllCreated(); 
    }
}