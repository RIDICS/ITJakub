using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Books.ViewModel
{
    internal class ErrorBarViewModel : ViewModelBase
    {
        private string m_message;

        public ErrorBarViewModel(string message)
        {
            Message = message;
            CloseCommand = new RelayCommand(Close);
        }

        public string Message
        {
            get { return m_message; }
            set { m_message = value; RaisePropertyChanged(); }
        }

        public RelayCommand CloseCommand { get; private set; }

        private async void Close()
        {
            await Task.Delay(1000);
            Messenger.Default.Send(new CloseErrorBarMessage());
        }
    }

    public class CloseErrorBarMessage { }
}