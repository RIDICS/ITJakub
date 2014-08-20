using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Core.Service.Polling
{
    public class PollingDispatcherTimer : IPollingTimer
    {
        private readonly IList<Action> m_actions;
        private readonly DispatcherTimer m_timer;

        public PollingDispatcherTimer(PollingInterval interval)
        {
            m_actions = new List<Action>();
            m_timer = new DispatcherTimer();
            m_timer.Interval = new TimeSpan((int) interval);
            m_timer.Tick += OnTimerTick;
        }

        public void Register(Action action)
        {
            m_actions.Add(action);

            if (!m_timer.IsEnabled)
                m_timer.Start();
        }

        public void Unregister(Action action)
        {
            m_actions.Remove(action);

            if (m_actions.Count == 0)
                m_timer.Stop();
        }

        private void OnTimerTick(object sender, object o)
        {
            foreach (var action in m_actions)
                action();
        }
    }
}