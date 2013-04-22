using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using ITJakubServerInterface;

namespace ChatMobileExample
{
    public partial class ChatControl : UserControl
    {
        private MobileServiceClient _mobileService;
        private ObservableCollection<String> _messages = new ObservableCollection<string>();
        private DispatcherTimer _timer = new DispatcherTimer();
        private long _chatSession;

        public ChatControl()
        {
            this.InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _mobileService = new MobileServiceClient("https://itjakub.azure-mobile.net/", "IKzmwpfkbiryIglFPmMRlsmAqwnLdY61");

            MessageList.ItemsSource = _messages;

            _chatSession = await FindChatSession();
            UpdateLoop();
        }

        private async void UpdateLoop()
        {
            for (; ; )
            {
                try
                {
                    await Task.Delay(1000);
                    var commands = _mobileService.GetTable<Command>();
                    var commandList = await commands.Where(command => command.SessionId == _chatSession).ToListAsync();
                    for (int i = _messages.Count; i < commandList.Count; i++)
                    {
                        _messages.Add(commandList[i].CommandText);
                    }
                }
                catch
                {
                }
            }
        }

        private async Task<long> FindChatSession()
        {
            var sessions = _mobileService.GetTable<Session>();
            var sessionList = await sessions.Where(session => session.Name == "ChatSession").ToListAsync();
            if (sessionList.Count == 0)
            {
                await sessions.InsertAsync(new Session() { Name = "ChatSession" });
                sessionList = await sessions.Where(session => session.Name == "ChatSession").ToListAsync();
            }
            long chatSession = sessionList[0].Id;
            return chatSession;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string text = NewMessageTextBox.Text;
            if (_chatSession != 0)
            {
                var commands = _mobileService.GetTable<Command>();
                commands.InsertAsync(new Command() { SessionId = _chatSession, CommandText = text });
            }
        }

    }
}
