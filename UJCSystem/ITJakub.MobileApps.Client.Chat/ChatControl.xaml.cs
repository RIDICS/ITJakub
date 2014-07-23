// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Chat.ViewModel;

namespace ITJakub.MobileApps.Client.Chat
{
    public sealed partial class ChatControl : UserControl
    {
        private ChatViewModel m_viewModel;

        public ChatControl()
        {
            InitializeComponent();
            m_viewModel = DataContext as ChatViewModel;
            if (m_viewModel != null)
            {
                m_viewModel.MessageHistory.CollectionChanged += ScrollToBottom;
            }
        }

        private async void ScrollToBottom(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MessageHistoryListView.Items == null) 
                return;

            var selectedIndex = MessageHistoryListView.Items.Count - 1;
            if (selectedIndex < 0)
                return;

            MessageHistoryListView.SelectedIndex = selectedIndex;
            MessageHistoryListView.UpdateLayout();
            await Task.Delay(50);
            MessageHistoryListView.ScrollIntoView(MessageHistoryListView.SelectedItem);
        }

        private void MessageBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            if (m_viewModel == null)
                return;

            m_viewModel.Message = MessageBox.Text;
            m_viewModel.SendCommand.Execute(null);
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Focus(FocusState.Keyboard);
        }
    }
}
