using System;
using System.Linq.Expressions;

namespace CachePower.WEB.Interfaces
{
    public interface IScheduler
    {
	    void Act(string id, Expression<Action> action, TimeSpan interval);
    }
}