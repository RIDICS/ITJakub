using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public abstract class ApplicationBaseViewModel : ViewModelBase
    {
        private bool m_dataLoaded;

        public abstract void InitializeCommunication(bool isUserOwner);
        public abstract void SetTask(string data);
        public abstract void EvaluateAndShowResults();
        public abstract void StopCommunication();
        
        public Action DataLoadedCallback { private get; set; }
        public Action GoBack { protected get; set; }

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

        public abstract IEnumerable<ActionViewModel> ActionsWithUsers { get; }
    }
}
