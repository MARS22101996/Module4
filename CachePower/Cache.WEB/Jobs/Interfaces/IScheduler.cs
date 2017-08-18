using System;
using System.Linq.Expressions;

namespace CachePower.WEB.Jobs.Interfaces
{
    public interface IScheduler
    {
	    void Act(string id, Expression<Action> action, TimeSpan interval);
    }
}