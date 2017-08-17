using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB
{
    public class SchedulerCreater : ISchedulerInitializer
    {
        private readonly IScheduler _scheduler;
        private readonly ICacheSettings _settings;
        private readonly IEnumerable<IJob> _jobs;

        public SchedulerCreater(IScheduler scheduler, ICacheSettings settings, IEnumerable<IJob> jobs)
        {
            _scheduler = scheduler;
            _settings = settings;
            _jobs = jobs;
        }

        public void Initialize()
        {
	        CheckIfUseUseWriteBehindStrategy();

	        CheckIfUseRefreshAheadStrategy();
        }

	    private void CheckIfUseUseWriteBehindStrategy()
	    {
			if (_settings.UseWriteBehindStrategy)
			{
				var job = _jobs.First(j => j.JobName.Equals("WriteBehindStrategy_Job"));

				_scheduler.RemoveIfExists(job.JobName);
			
				_scheduler.AddOrUpdate(job.JobName, () => job.Execute(), TimeSpan.FromMinutes(_settings.WriteBehindSyncInterval));
			}
		}

	    private void CheckIfUseRefreshAheadStrategy()
	    {
			if (_settings.UseRefreshAheadStrategy)
			{
				var job = _jobs.First(j => j.JobName.Equals("RefreshAheadStrategy"));

				_scheduler.RemoveIfExists(job.JobName);

				_scheduler.AddOrUpdate(job.JobName, () => job.Execute(), TimeSpan.FromMinutes(_settings.UpdateExpirationInterval));
			}
		}


	}
}