// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

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
