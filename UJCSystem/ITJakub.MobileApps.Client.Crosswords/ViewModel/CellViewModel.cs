using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        private char m_letter;

        public char Letter
        {
            get { return m_letter; }
            set
            {
                m_letter = value;
                RaisePropertyChanged();
            }
        }

        public bool IsPartOfAnswer { get; set; }
    }
}