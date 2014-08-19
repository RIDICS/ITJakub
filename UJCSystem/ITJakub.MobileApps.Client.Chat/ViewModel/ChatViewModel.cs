using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Chat.DataService;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Helpers;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    public class ChatViewModel : ApplicationBaseViewModel
    {
        private readonly IChatDataService m_dataService;
        private readonly RelayCommand m_sendCommand;
        private string m_message;
        private ObservableCollection<MessageViewModel> m_messageHistory;
        private DispatcherTimer m_timer;

        /// <summary>
        /// Initializes a new instance of the ChatViewModel class.
        /// </summary>
        public ChatViewModel(IChatDataService dataService)
        {
            m_dataService = dataService;
            m_sendCommand = new RelayCommand(SendMessage);

            MessageHistory = new AsyncObservableCollection<MessageViewModel>();
        }

        public override void InitializeCommunication()
        {
            MessageHistory.Clear();
            m_timer = new DispatcherTimer();
            m_timer.Interval = new TimeSpan(0, 0, 0, 1);
            m_timer.Tick += LoadNewMessages;

            m_dataService.GetAllChatMessages((messages, exception) =>
            {
                if (exception != null)
                    return;

                foreach(var message in messages)
                {
                    MessageHistory.Add(message);
                }
                m_timer.Start();
            });
        }

        public RelayCommand SendCommand
        {
            get { return m_sendCommand; }
        }

        public string Message
        {
            get { return m_message; }
            set
            {
                m_message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public ObservableCollection<MessageViewModel> MessageHistory
        {
            get { return m_messageHistory; }
            set
            {
                m_messageHistory = value;
                RaisePropertyChanged(() => MessageHistory);
            }
        }

        private void SendMessage()
        {
            if (Message == string.Empty)
                return;

            m_dataService.SendMessage(Message, exception =>
            {
                if (exception != null)
                    return;

                //TODO if refresh interval is long then LoadNewMessages immediately
            });
            
            Message = string.Empty;
        }

        private void LoadNewMessages(object sender, object o)
        {
            var since = MessageHistory.Count > 0
                ? MessageHistory[MessageHistory.Count - 1].SendTime
                : new DateTime(1970, 1, 1);

            m_dataService.GetChatMessages(since, (messages, exception) =>
            {
                if (exception != null)
                    return;

                foreach (var message in messages)
                {
                    MessageHistory.Add(message);
                }
            });
        }

        public override void StopTimers()
        {
            m_timer.Stop();
            m_timer.Tick -= LoadNewMessages;
        }
    }
}