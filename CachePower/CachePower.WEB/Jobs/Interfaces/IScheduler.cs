using System;
using System.Linq.Expressions;

namespace CachePower.WEB.Interfaces
{
    public interface IScheduler
    {
        void Add(string id, Expression<Action> action, TimeSpan interval);

        void Delete(string id);
    }
}