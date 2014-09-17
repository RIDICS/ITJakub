using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordRowViewModel : ViewModelBase
    {
        private string m_word;

        public CrosswordRowViewModel(string label, int wordLength, int startPosition, int answerColumn, int rowIndex)
        {
            Label = label;
            StartPosition = startPosition;
            RowIndex = rowIndex;
            AnswerPosition = answerColumn - startPosition;
            CreateCells(wordLength);
        }

        public int RowIndex { get; private set; }

        public string Label { get; private set; }

        public ObservableCollection<CellViewModel> Letters { get; private set; }

        public int StartPosition { get; private set; }

        public string Word
        {
            get { return m_word; }
            set
            {
                m_word = value.ToUpper();
                UpdateCells(m_word);
                FillWordAction(this);
            }
        }

        public Action<CrosswordRowViewModel> FillWordAction { get; set; }
        
        public int AnswerPosition { get; private set; }

        public void UpdateWord(string word)
        {
            m_word = word.ToUpper();
            UpdateCells(m_word);
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
            
            Letters[AnswerPosition].IsPartOfAnswer = true;
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
}