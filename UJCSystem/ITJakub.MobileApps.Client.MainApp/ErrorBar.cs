using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;

namespace ITJakub.MobileApps.Client.MainApp
{
    public class ErrorBar
    {
        private readonly string m_message;
        private Popup m_popup;

        public ErrorBar(string message)
        {
            m_message = message;
        }

        public void Show()
        {
            m_popup = new Popup();
            var viewModel = new ErrorBarViewModel(m_message);
            var errorBarView = new ErrorBarView
            {
                DataContext = viewModel,
                Width = Window.Current.Bounds.Width
            };

            m_popup.Child = errorBarView;
            m_popup.IsOpen = true;

            Messenger.Default.Register<CloseErrorBarMessage>(this, Close);
        }

        private void Close(CloseErrorBarMessage message)
        {
            m_popup.IsOpen = false;
            Messenger.Default.Unregister(this);
        }
    }
}
