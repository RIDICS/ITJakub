using System;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBaseViewModel : ViewModelBase
    {
        private bool m_dataLoaded;

        public abstract void InitializeCommunication();
        public abstract void SetTask(string data);
        public abstract void StopCommunication();

        public Action DataLoadedCallback { private get; set; }

        protected void SetDataLoaded()
        {
            if (m_dataLoaded)
                return;

            m_dataLoaded = true;

            if (DataLoadedCallback == null)
                return;

            DataLoadedCallback();
            DataLoadedCallback = null;
        }
    }
}
