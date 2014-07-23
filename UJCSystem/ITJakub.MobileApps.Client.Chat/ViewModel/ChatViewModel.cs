using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.DataService;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChatViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly RelayCommand m_sendCommand;
        private string m_message;
        private ObservableCollection<MessageViewModel> m_messageHistory;

        /// <summary>
        /// Initializes a new instance of the ChatViewModel class.
        /// </summary>
        public ChatViewModel(IDataService dataService)
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
                Content = Message,
                DateTime = DateTime.Now,
                Name = "Já",
                IsMyMessage = true
            });
            MessageHistory.Add(new MessageViewModel
            {
                Content = "Reply",
                DateTime = DateTime.Now,
                Name = "User",
                IsMyMessage = false
            });

            Message = string.Empty;
        }
    }
}