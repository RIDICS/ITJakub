using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class TimerService : ITimerService
    {
        private readonly Dictionary<PollingInterval, IPollingTimer> m_pollingTimers;

        public TimerService()
        {
            m_pollingTimers = new Dictionary<PollingInterval, IPollingTimer>();
        }

        public void Register(PollingInterval interval, Action action)
        {
            if (!m_pollingTimers.ContainsKey(interval))
            {
                m_pollingTimers.Add(interval, new PollingDispatcherTimer(interval));
            }
            
            m_pollingTimers[interval].Register(action);
        }

        public void Unregister(PollingInterval interval, Action action)
        {
            if (!m_pollingTimers.ContainsKey(interval))
                return;

            m_pollingTimers[interval].Unregister(action);
        }
    }
}
