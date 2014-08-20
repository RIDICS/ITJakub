using System;
using System.Collections.Generic;
using Windows.System.Threading;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingBackgroundTimer : IPollingTimer
    {
        private readonly IList<Action> m_actions;
        private ThreadPoolTimer m_timer;
        private TimeSpan m_timeSpan;

        public PollingBackgroundTimer(PollingInterval interval)
        {
            m_timeSpan = new TimeSpan(0, 0, (int) interval);
            m_actions = new List<Action>();
        }

        public void Register(Action action)
        {
            m_actions.Add(action);

            if (m_timer == null)
                ThreadPoolTimer.CreateTimer(OnTimerTick, m_timeSpan);
        }

        public void Unregister(Action action)
        {
            m_actions.Remove(action);
        }

        private void OnTimerTick(ThreadPoolTimer timer)
        {
            if (m_actions.Count == 0)
                return;

            foreach (var action in m_actions)
                action();
        }
    }
}