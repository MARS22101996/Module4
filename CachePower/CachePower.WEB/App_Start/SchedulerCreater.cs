using System;
using System.Collections.Generic;
using System.Linq;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB
{
    public class SchedulerCreater : ISchedulerConfigurer
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

        public void Configure()
        {
	        CheckIfUseUseWriteBehindStrategy();

	        CheckIfUseRefreshAheadStrategy();
        }

	    private void CheckIfUseUseWriteBehindStrategy()
	    {
		    if (!_settings.UseWriteBehindStrategy) return;

		    var job = _jobs.First(j => j.Name.Equals("WriteBehindStrategy_Job"));

		    _scheduler.Act(job.Name, () => job.Run(), TimeSpan.FromMinutes(_settings.WriteBehindSyncInterval));
	    }

	    private void CheckIfUseRefreshAheadStrategy()
	    {
		    if (!_settings.UseRefreshAheadStrategy) return;

		    var job = _jobs.First(j => j.Name.Equals("RefreshAheadStrategy"));

		    _scheduler.Act(job.Name, () => job.Run(), TimeSpan.FromMinutes(_settings.UpdateExpirationInterval));
	    }
	}
}