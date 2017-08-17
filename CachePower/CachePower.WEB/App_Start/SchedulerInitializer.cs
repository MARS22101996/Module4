using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CachePower.DAL.Interfaces;
using CachePower.WEB.Interfaces;

namespace CachePower.WEB
{
    public class SchedulerInitializer : ISchedulerInitializer
    {
        private readonly IScheduler _scheduler;
        private readonly ICacheSettings _settings;
        private readonly IEnumerable<IJob> _jobs;

        public SchedulerInitializer(IScheduler scheduler, ICacheSettings settings, IEnumerable<IJob> jobs)
        {
            _scheduler = scheduler;
            _settings = settings;
            _jobs = jobs;
        }

        public void Initialize()
        {
            if (_settings.UseWriteBehindStrategy)
            {
                var job = _jobs.First(j => j.JobName.Equals("Save_To_Database"));

                _scheduler.RemoveIfExists(job.JobName);
                Expression<Action> action = () => job.Execute();
                _scheduler.AddOrUpdate(job.JobName, action, TimeSpan.FromMinutes(_settings.WriteBehindSyncInterval));
            }

            if (_settings.UseRefreshAheadStrategy)
            {
                var job = _jobs.First(j => j.JobName.Equals("Update_Expirations"));

                _scheduler.RemoveIfExists(job.JobName);
                Expression<Action> action = () => job.Execute();
                _scheduler.AddOrUpdate(job.JobName, action, TimeSpan.FromMinutes(_settings.UpdateExpirationInterval));
            }
        }
    }
}