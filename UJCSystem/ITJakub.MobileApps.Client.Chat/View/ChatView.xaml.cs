// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Chat.View
{
    public sealed partial class ChatView
    {
        public ChatView()
        {
            InitializeComponent();
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Focus(FocusState.Keyboard);
        }
    }
}
