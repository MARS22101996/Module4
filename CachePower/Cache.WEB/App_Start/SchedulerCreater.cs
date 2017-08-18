using System;
using Cache.DAL.Repositories.Interfaces;
using Cache.WEB.Jobs.Concrete;
using Cache.WEB.Jobs.Interfaces;

namespace Cache.WEB
{
    public class SchedulerCreater : ISchedulerConfigurer
    {
        private readonly IJobScheduler _jobScheduler;
        private readonly ICacheSettings _settings;
        private  TimeSpan _timeSpan;

        public SchedulerCreater(IJobScheduler jobScheduler, ICacheSettings settings)
        {
            _jobScheduler = jobScheduler;
            _settings = settings;
        }

        public void Configure()
        {
			CheckIfUseUseWriteBehindStrategy();

			CheckIfUseRefreshAheadStrategy();
        }

	    private void CheckIfUseUseWriteBehindStrategy()
	    {
		    if (!_settings.IsUseWriteBehindStrategy) return;

		    var job = new  WriteBehindStrategyJob();

		    _timeSpan = TimeSpan.FromMinutes(_settings.WriteBehindSyncInterval);

			_jobScheduler.Act(typeof(WriteBehindStrategyJob).Name, () => job.Run(), _timeSpan);
	    }

	    private void CheckIfUseRefreshAheadStrategy()
	    {
		    if (!_settings.IsRefreshAheadStrategy) return;  

			var job = new RefreshAheadStrategyJob();

		    _timeSpan = TimeSpan.FromMinutes(_settings.UpdateExpiration);

			_jobScheduler.Act(typeof(RefreshAheadStrategyJob).Name, () => job.Run(), _timeSpan);
	    }
	}
}