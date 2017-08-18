using System;
using System.Linq.Expressions;

namespace Cache.WEB.Jobs.Interfaces
{
    public interface IJobScheduler
    {
	    void Act(string id, Expression<Action> action, TimeSpan interval);
    }
}