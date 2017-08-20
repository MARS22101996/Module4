using System;
using System.Linq.Expressions;
using Cache.WEB.Jobs.Interfaces;
using Hangfire;

namespace Cache.WEB.Jobs
{
    public class JobScheduler : IJobScheduler
    {
	    public void Act(string id, Expression<Action> action, TimeSpan interval)
	    {
			RecurringJob.RemoveIfExists(id);

			RecurringJob.AddOrUpdate(id, action, Cron.MinuteInterval((int)interval.TotalMinutes));
		}
    }
}