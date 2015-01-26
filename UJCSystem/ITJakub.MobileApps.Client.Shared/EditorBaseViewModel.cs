using System;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class EditorBaseViewModel : ViewModelBase
    {
        private bool m_saving;

        public Action GoBack { get; set; }

        public bool Saving
        {
            get { return m_saving; }
            set
            {
                m_saving = value;
                RaisePropertyChanged();
            }
        }
    }
}
