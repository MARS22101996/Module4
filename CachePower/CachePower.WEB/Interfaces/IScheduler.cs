using System;
using System.Linq.Expressions;

namespace CachePower.WEB.Interfaces
{
    public interface IScheduler
    {
        void AddOrUpdate(string jobId, Expression<Action> action, TimeSpan interval);

        void RemoveIfExists(string jobId);
    }
}