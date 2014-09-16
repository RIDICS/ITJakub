using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordRowViewModel : ViewModelBase
    {
        private string m_word;
        private readonly int m_answerPosition;

        public CrosswordRowViewModel(string label, int wordLength, int startPosition, int answerColumn, int rowIndex)
        {
            Label = label;
            StartPosition = startPosition;
            RowIndex = rowIndex;
            m_answerPosition = answerColumn - startPosition;
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
                RaisePropertyChanged();
                UpdateCells(m_word);
                FillWordAction(this);
            }
        }

        public Action<CrosswordRowViewModel> FillWordAction { get; set; }

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
}