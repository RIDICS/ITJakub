using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanTaskPreviewViewModel : TaskPreviewBaseViewModel
    {
        private string m_test;

        public override void ShowTask(string data)
        {
            Test = data;
        }

        public string Test
        {
            get { return m_test; }
            set { m_test = value; RaisePropertyChanged(); }
        }
    }
}