using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caghing.Dal.Entities;
using Caghing.Dal.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Configuration;

namespace Caghing.Dal.Repositories
{
    public class CargoCachedRepository : ICargoCachedRepository
    {

        private readonly IDatabase _cache;
        private readonly ICargoRepository _repository;

        public CargoCachedRepository(IDatabase cache, ICargoRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public Cargo GetById(int id)
        {
            Cargo cargo;
            var cachedValue = _cache.StringGet(nameof(Cargo) + "_" + id);

            if (!cachedValue.IsNull)
            {
                cargo = JsonConvert.DeserializeObject<Cargo>(cachedValue.ToString());
            }
            else
            {
                cargo = _repository.GetById(id);

                if (cargo != null)
                {
                    _cache.StringSet(nameof(Cargo) + "_" + id, JsonConvert.SerializeObject(cargo));
                }
            }

            return cargo;
        }

        public void Create(Cargo item)
        {
            _repository.Create(item);
        }
    }
}
