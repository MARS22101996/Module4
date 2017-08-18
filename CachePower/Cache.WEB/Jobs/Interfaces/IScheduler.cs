using System;
using System.Linq.Expressions;

namespace Cache.WEB.Jobs.Interfaces
{
    public interface IScheduler
    {
	    void Act(string id, Expression<Action> action, TimeSpan interval);
    }
}