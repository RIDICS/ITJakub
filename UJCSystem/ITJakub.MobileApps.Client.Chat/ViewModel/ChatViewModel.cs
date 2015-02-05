using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Chat.DataService;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Control;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Chat.ViewModel
{
    public class ChatViewModel : ApplicationBaseViewModel
    {
        private readonly IChatDataService m_dataService;
        private readonly RelayCommand m_sendCommand;
        private string m_message;
        private ObservableCollection<MessageViewModel> m_messageHistory;
        private bool m_loading;

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
            Loading = true;
            DataLoadedCallback = () => Loading = false;

            m_dataService.StartChatMessagesPolling((messages, exception) =>
            {
                if (exception != null)
                    return;

                foreach (var message in messages)
                {
                    MessageHistory.Add(message);
                }

                SetDataLoaded();
            });
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

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }
    }
}