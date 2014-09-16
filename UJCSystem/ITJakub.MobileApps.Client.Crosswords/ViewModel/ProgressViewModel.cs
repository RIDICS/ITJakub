using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class ProgressViewModel
    {
        public ObservableCollection<RowProgressViewModel> Rows { get; set; }

        public AuthorInfo UserInfo { get; set; }
    }

    public class RowProgressViewModel
    {
        private int m_answerPosition;
        private int m_filledLength;

        public RowProgressViewModel(int wordLength, int startPosition, int answerColumn)
        {
            Filled = new ObservableCollection<bool>();
            StartPosition = startPosition;
            m_answerPosition = answerColumn - startPosition;
            
            for (int i = 0; i < wordLength; i++)
                Filled.Add(false);
        }

        public int StartPosition { get; set; }

        public ObservableCollection<bool> Filled { get; set; }

        public bool IsCorrect { get; set; }

        public int FilledLength
        {
            get { return m_filledLength; }
            set
            {
                m_filledLength = value;
                for (int i = 0; i < m_filledLength; i++)
                    Filled[i] = true;
                for (int i = m_filledLength; i < Filled.Count; i++)
                    Filled[i] = false;
            }
        }
    }
}