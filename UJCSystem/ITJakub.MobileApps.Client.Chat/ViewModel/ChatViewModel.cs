using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Chat.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    public class ChatViewModel : ApplicationBaseViewModel
    {
        private readonly IChatDataService m_dataService;
        private readonly RelayCommand m_sendCommand;
        private string m_message;
        private ObservableCollection<MessageViewModel> m_messageHistory;

        /// <summary>
        /// Initializes a new instance of the ChatViewModel class.
        /// </summary>
        public ChatViewModel(IChatDataService dataService)
        {
            m_dataService = dataService;
            m_sendCommand = new RelayCommand(SendMessage);
            MessageHistory = new ObservableCollection<MessageViewModel>();
            Initialize();
        }

        private void Initialize()
        {
            MessageHistory.Clear();
            m_dataService.GetAllChatMessages((messages, exception) =>
            {
                if (exception != null)
                    return;
                foreach(var message in messages)
                {
                    MessageHistory.Add(message);
                }
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

            //TODO send message to server and remove this method
            MessageHistory.Add(new MessageViewModel
            {
                Text = Message,
                SendTime = DateTime.Now,
            });

            Message = string.Empty;
        }
    }
}