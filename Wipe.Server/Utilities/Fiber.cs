﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wipe.MMO.Utilities
{
    /// <summary>
    /// A synchronised fiber that uses the default thread pool and TPL
    /// </summary>
    public class Fiber
    {

        private ConcurrentExclusiveSchedulerPair m_schedulers = new ConcurrentExclusiveSchedulerPair();

        private CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Enqueue a function
        /// </summary>
        /// <param name="f">The function to enqueue</param>
        /// <param name="exclusive">Should the function run exclusively?</param>
        public Task<T> Enqueue<T>(Func<T> f, bool exclusive = true)
        {
            Task<T> task = new Task<T>(f);

            Start(task, exclusive);

            return task;
        }

        /// <summary>
        /// Enqueue a function
        /// </summary>
        /// /// <param name="f">The function to enqueue</param>
        /// <param name="exclusive">Should the function run exclusively?</param>
        public Task Enqueue(Action f, bool exclusive = true)
        {
            Task task = new Task(f);

            Start(task, exclusive);

            return task;
        }

        /// <summary>
        /// Schedule a function to be enqueued in the future
        /// </summary>
        /// <param name="f">The function to schedule</param>
        /// <param name="waitTime">How long to wait before enqueueing the function</param>
        /// <param name="exclusive">Should the function run exclusively?</param>
        public Task Schedule(Action f, TimeSpan waitTime, bool exclusive = true)
        {
            return Task.Delay(waitTime, m_cancellationTokenSource.Token).ContinueWith((t) => Enqueue(f, exclusive), TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();
        }

        /// <summary>
        /// Schedule a function to be enqueued in the future
        /// </summary>
        /// <param name="f">The function to schedule</param>
        /// <param name="waitTime">How long to wait before enqueueing the function</param>
        /// <param name="exclusive">Should the function run exclusively?</param>
        public Task<T> Schedule<T>(Func<T> f, TimeSpan waitTime, bool exclusive = true)
        {
            return Task.Delay(waitTime, m_cancellationTokenSource.Token).ContinueWith((t) => Enqueue<T>(f, exclusive), TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();
        }

        public void Stop()
        {
            m_schedulers.Complete();
        }

        private void Start(Task t, bool exclusive)
        {
            try
            {
                t.Start(exclusive ? m_schedulers.ExclusiveScheduler : m_schedulers.ConcurrentScheduler);
            }
            catch (TaskSchedulerException ex)
            {
                //InvalidOperationException will throw if the fiber is stopped but something tries to enqueue another job
                if (!(ex.InnerException is InvalidOperationException))
                {
                    //s_log.Warn("Exception starting task on fiber: {0}", ex);
                }
            }
        }
    }
}
