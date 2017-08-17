﻿using System;
using System.Linq.Expressions;
using CachePower.WEB.Interfaces;
using Hangfire;

namespace CachePower.WEB.Schedulers
{
    public class HangfireScheduler : IScheduler
    {
        public void Add(string jobId, Expression<Action> action, TimeSpan interval)
        {
            RecurringJob.AddOrUpdate(jobId, action, Cron.MinuteInterval((int)interval.TotalMinutes));
        }

        public void Delete(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}