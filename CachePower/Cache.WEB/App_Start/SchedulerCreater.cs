﻿using System;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Concrete;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB
{
    public class SchedulerCreater : ISchedulerConfigurer
    {
        private readonly IScheduler _scheduler;
        private readonly ICacheSettings _settings;
        private  TimeSpan _timeSpan;

        public SchedulerCreater(IScheduler scheduler, ICacheSettings settings)
        {
            _scheduler = scheduler;
            _settings = settings;
        }

        public void Configure()
        {
			CheckIfUseUseWriteBehindStrategy();

			CheckIfUseRefreshAheadStrategy();
        }

	    private void CheckIfUseUseWriteBehindStrategy()
	    {
		    if (!_settings.UseWriteBehindStrategy) return;

		    var job = new  WriteBehindStrategyJob();

		    _timeSpan = TimeSpan.FromMinutes(_settings.WriteBehindSyncInterval);

			_scheduler.Act(job.Name.ToString(), () => job.Run(), _timeSpan);
	    }

	    private void CheckIfUseRefreshAheadStrategy()
	    {
		    if (!_settings.UseRefreshAheadStrategy) return;  

			var job = new RefreshAheadStrategyJob();

		    _timeSpan = TimeSpan.FromMinutes(_settings.UpdateExpirationInterval);

			_scheduler.Act(job.Name.ToString(), () => job.Run(), _timeSpan);
	    }
	}
}