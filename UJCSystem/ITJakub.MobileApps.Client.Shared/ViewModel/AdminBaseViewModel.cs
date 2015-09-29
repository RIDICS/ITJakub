using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public abstract class AdminBaseViewModel : ViewModelBase
    {
        private bool m_dataLoaded;

        public abstract void SetTask(string data);
        public abstract void InitializeCommunication();
        public abstract void UpdateGroupMembers(IEnumerable<UserInfo> members);

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