// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Chat.View
{
    public sealed partial class ChatView
    {
        public ChatView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var items = MessageHistoryListView.Items;
            if (items == null) 
                return;

            items.VectorChanged += ScrollToBottom;
            ScrollToBottom();
        }

        private async void ScrollToBottom()
        {
            await Task.Delay(100);
            ScrollToBottom(MessageHistoryListView.Items, null);
        }

        private async void ScrollToBottom(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var selectedIndex = sender.Count - 1;
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

            var binding = MessageBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null) 
                binding.UpdateSource();

            var sendCommand = SendButton.Command;
            if (sendCommand != null && sendCommand.CanExecute(null))
                sendCommand.Execute(null);
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Focus(FocusState.Keyboard);
        }
    }
}
