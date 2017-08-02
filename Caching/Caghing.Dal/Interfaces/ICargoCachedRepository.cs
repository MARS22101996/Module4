using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caghing.Dal.Entities;

namespace Caghing.Dal.Interfaces
{
    public interface ICargoCachedRepository
    {
        void Create(Cargo item);

        Cargo GetById(int id);
    }
}
