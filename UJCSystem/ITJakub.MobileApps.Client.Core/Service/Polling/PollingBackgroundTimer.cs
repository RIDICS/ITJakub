using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingBackgroundTimer : IPollingTimer
    {
        private readonly IList<Action> m_actions;
        private ThreadPoolTimer m_timer;
        private readonly TimeSpan m_timeSpan;

        public PollingBackgroundTimer(PollingInterval interval)
        {
            m_timeSpan = new TimeSpan(0, 0, (int) interval);
            m_actions = new List<Action>();
        }

        public void Register(Action action)
        {
            lock (this)
            {
                m_actions.Add(action);

                if (m_timer == null)
                    m_timer = ThreadPoolTimer.CreateTimer(OnTimerTick, m_timeSpan);
            }
        }

        public void Unregister(Action action)
        {
            lock (this)
            {
                m_actions.Remove(action);   
            }
        }

        private void OnTimerTick(ThreadPoolTimer timer)
        {
            if (m_actions.Count == 0)
            {
                lock (this)
                {
                    if (m_actions.Count == 0)
                    {
                        m_timer = null;
                        return;   
                    }
                }
            }

            var actions = new List<Action>(m_actions);
            var remainingActions = actions.Count;

            foreach (var action in actions)
            {
                var task = new Task(action);
                task.ContinueWith(task1 =>
                {
                    Interlocked.Decrement(ref remainingActions);
                    if (remainingActions == 0)
                        m_timer = ThreadPoolTimer.CreateTimer(OnTimerTick, m_timeSpan);
                });
                task.Start();
            }
        }
    }
}