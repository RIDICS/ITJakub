using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Chat.DataService;
using ITJakub.MobileApps.Client.Chat.Message;
using ITJakub.MobileApps.Client.Shared.Control;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    public class ChatViewModel : SupportAppBaseViewModel
    {
        private readonly IChatDataService m_dataService;
        private readonly RelayCommand m_sendCommand;
        private string m_message;
        private ObservableCollection<MessageViewModel> m_messageHistory;
        private bool m_loading;

        public ChatViewModel(IChatDataService dataService)
        {
            m_dataService = dataService;
            m_sendCommand = new RelayCommand(SendMessage);

            MessageHistory = new AsyncObservableCollection<MessageViewModel>();
        }

        public override void InitializeCommunication()
        {
            Loading = true;
            DataLoadedCallback = () => Loading = false;

            m_dataService.StartChatMessagesPolling(ProcessNewMessages);
        }

        public override void SetTask(string data) { }

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

        public bool Loading
        {
            get { return m_loading; }
            set
            {
                m_loading = value;
                RaisePropertyChanged();
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
            });
            
            Message = string.Empty;
        }

        private void ProcessNewMessages(ObservableCollection<MessageViewModel> messages, Exception exception)
        {
            if (exception != null)
                return;

            if (messages.Count > 0)
                MessengerInstance.Send(new NotifyNewMessagesMessage { Count = messages.Count });

            foreach (var message in messages)
            {
                MessageHistory.Add(message);
            }

            SetDataLoaded();
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get { return new ActionViewModel[0]; }
        }

        public override void AppVisibilityChanged(bool isVisible)
        {
            var newPollingInterval = isVisible ? ChatManager.FastPollingInterval : ChatManager.SlowPollingInterval;
            m_dataService.UpdateMessagePollingInterval(newPollingInterval);
        }
    }
}