using System;
using System.Linq.Expressions;

namespace Cache.WEB.Interfaces
{
    public interface IScheduler
    {
	    void Act(string id, Expression<Action> action, TimeSpan interval);
    }
}