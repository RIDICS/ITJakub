using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Crosswords
{
    public class SimpleCrosswordsViewModel : ViewModelBase
    {
        private ObservableCollection<CrosswordRowViewModel> m_crossword;

        public SimpleCrosswordsViewModel()
        {
            const int answerColumn = 4;
            Crossword = new ObservableCollection<CrosswordRowViewModel>
            {
                new CrosswordRowViewModel("První zadání", 7, 1, answerColumn),
                new CrosswordRowViewModel("Druhé", 5, 0, answerColumn),
                new CrosswordRowViewModel("Nìjaké dlouhé zadání, tak aby pokraèoval i na dalším øádku", 8, 4, answerColumn),
                new CrosswordRowViewModel("Hádej", 7, 1, answerColumn),
            };
        }

        public ObservableCollection<CrosswordRowViewModel> Crossword
        {
            get { return m_crossword; }
            set
            {
                m_crossword = value;
                RaisePropertyChanged();
            }
        }
    }

    public class CrosswordRowViewModel : ViewModelBase
    {
        private string m_word;
        private readonly int m_answerPosition;

        public CrosswordRowViewModel(string label, int wordLength, int startPosition, int answerColumn)
        {
            Label = label;
            StartPosition = startPosition;
            m_answerPosition = answerColumn - startPosition;
            CreateCells(wordLength);
            Word = string.Empty;
        }

        public string Label { get; private set; }

        public ObservableCollection<CellViewModel> Letters { get; private set; }

        public int StartPosition { get; private set; }

        public string Word
        {
            get { return m_word; }
            set
            {
                m_word = value;
                RaisePropertyChanged();
                UpdateCells(m_word);
            }
        }

        private void UpdateCells(string word)
        {
            if (word.Length > Letters.Count)
                return;

            var newLetters = word.ToCharArray();

            for (int i = 0; i < newLetters.Length; i++)
            {
                Letters[i].Letter = newLetters[i];
            }
            for (int i = word.Length; i < Letters.Count; i++)
            {
                Letters[i].Letter = ' ';
            }
        }

        private void CreateCells(int cellCount)
        {
            Letters = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < cellCount; i++)
            {
                Letters.Add(new CellViewModel());
            }
            
            Letters[m_answerPosition].IsPartOfAnswer = true;
        }
    }

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