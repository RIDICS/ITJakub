using System;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Core.ViewModel.News
{
    public class SyndicationItemViewModel : ViewModelBase
    {
        private DateTime m_createDate;
        private string m_text;
        private string m_title;
        private string m_url;
        private string m_userEmail;
        private string m_userFirstName;
        private string m_userLastName;

        public string Title
        {
            get { return m_title; }
            set
            {
                m_title = value;
                RaisePropertyChanged();
            }
        }

        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                RaisePropertyChanged();
            }
        }

        public string Url
        {
            get { return m_url; }
            set
            {
                m_url = value;
                RaisePropertyChanged();
            }
        }

        public DateTime CreateDate
        {
            get { return m_createDate; }
            set
            {
                m_createDate = value;
                RaisePropertyChanged();
            }
        }

        public string UserEmail
        {
            get { return m_userEmail; }
            set
            {
                m_userEmail = value;
                RaisePropertyChanged();
            }
        }

        public string UserFirstName
        {
            get { return m_userFirstName; }
            set
            {
                m_userFirstName = value;
                RaisePropertyChanged();
            }
        }

        public string UserLastName
        {
            get { return m_userLastName; }
            set
            {
                m_userLastName = value;
                RaisePropertyChanged();
            }
        }
    }
}