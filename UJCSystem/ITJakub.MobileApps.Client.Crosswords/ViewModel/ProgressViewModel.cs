using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class ProgressViewModel
    {
        public ObservableCollection<RowProgressViewModel> Rows { get; set; }

        public UserInfo UserInfo { get; set; }
    }

    public class RowProgressViewModel
    {
        private int m_filledLength;

        public RowProgressViewModel()
        {
        }

        public RowProgressViewModel(int rowIndex, int wordLength, int startPosition, int answerPosition)
        {
            Cells = new CellViewModel[wordLength];
            StartPosition = startPosition;
            RowIndex = rowIndex;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new CellViewModel();
            }

            Cells[answerPosition].IsPartOfAnswer = true;
        }

        public int RowIndex { get; private set; }

        public CellViewModel[] Cells { get; set; }

        public int StartPosition { get; set; }

        public bool IsCorrect { get; set; }

        public int FilledLength
        {
            get { return m_filledLength; }
            set
            {
                m_filledLength = value;
                for (int i = 0; i < m_filledLength; i++)
                    Cells[i].IsFilled = true;
                for (int i = m_filledLength; i < Cells.Length; i++)
                    Cells[i].IsFilled = false;
            }
        }

        public class CellViewModel : ViewModelBase
        {
            private bool m_isFilled;

            public bool IsPartOfAnswer { get; set; }

            public bool IsFilled
            {
                get { return m_isFilled; }
                set
                {
                    m_isFilled = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}