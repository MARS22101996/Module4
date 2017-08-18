using System;
using System.Linq.Expressions;
using Cache.WEB.Interfaces;
using Hangfire;

namespace Cache.WEB.Schedulers
{
    public class Scheduler : IScheduler
    {
	    public void Act(string jobId, Expression<Action> action, TimeSpan interval)
	    {
			RecurringJob.RemoveIfExists(jobId);

			RecurringJob.AddOrUpdate(jobId, action, Cron.MinuteInterval((int)interval.TotalMinutes));
		}
    }
}