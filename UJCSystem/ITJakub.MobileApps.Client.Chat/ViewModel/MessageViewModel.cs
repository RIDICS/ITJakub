using System;
using Windows.UI;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        private AuthorInfo m_author;
        private string m_text;
        private Color m_messageBackground;
        private DateTime m_sendTime;

        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                RaisePropertyChanged();
            }
        }

        public AuthorInfo Author
        {
            get { return m_author; }
            set
            {
                m_author = value;
                RaisePropertyChanged();
            }
        }

        public Color MessageBackground
        {
            get { return m_messageBackground; }
            set { m_messageBackground = value; RaisePropertyChanged();}
        }

        public DateTime SendTime
        {
            get { return m_sendTime; }
            set { m_sendTime = value; RaisePropertyChanged();}
        }

        public bool IsMyMessage { get; set; }
    }
}