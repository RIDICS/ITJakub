using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ErrorBarViewModel : ViewModelBase
    {
        private string m_message;

        /// <summary>
        /// Initializes a new instance of the ErrorBarViewModel class.
        /// </summary>
        /// <param name="message"></param>
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