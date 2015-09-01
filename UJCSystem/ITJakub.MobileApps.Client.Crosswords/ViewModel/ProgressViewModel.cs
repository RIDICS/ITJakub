using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class ProgressViewModel : ViewModelBase
    {
        private DateTime? m_firstTime;
        private bool m_win;
        private int m_letterCount;

        public ProgressViewModel(UserInfo userInfo, DateTime? firstTime)
        {
            m_firstTime = firstTime;
            UserInfo = userInfo;
        }

        public ObservableCollection<RowProgressViewModel> Rows { get; set; }

        public UserInfo UserInfo { get; set; }

        public TimeSpan GameTime { get; private set; }

        public bool Win
        {
            get { return m_win; }
            set
            {
                m_win = value;
                RaisePropertyChanged();
            }
        }

        public int LetterCount
        {
            get { return m_letterCount; }
            set
            {
                m_letterCount = value;
                RaisePropertyChanged();
            }
        }

        //public void UpdateRowInfo(int row, bool isCorrect)
        //{
        //    m_correctAnswers[row] = isCorrect;
        //    LetterCount = m_correctAnswers.Count(b => b);
        //    Win = m_correctAnswers.All(correctAnswer => correctAnswer);
        //}

        public void UpdateTime(DateTime time)
        {
            if (m_firstTime == null)
                m_firstTime = time;

            GameTime = time - m_firstTime.Value;
        }
    }

    public class RowProgressViewModel : ViewModelBase
    {
        private int m_filledLength;
        private bool m_isCorrect;

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

        public bool IsCorrect
        {
            get { return m_isCorrect; }
            set
            {
                m_isCorrect = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => Cells);
            }
        }

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