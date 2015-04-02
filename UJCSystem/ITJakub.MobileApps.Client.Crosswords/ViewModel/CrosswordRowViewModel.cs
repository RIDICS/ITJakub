using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordRowViewModel : ViewModelBase
    {
        private string m_word;
        private bool? m_isCorrect;

        public CrosswordRowViewModel()
        {
            
        }

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

        public ObservableCollection<CellViewModel> Cells { get; private set; }

        public int StartPosition { get; private set; }

        public string Word
        {
            get { return m_word; }
            set
            {
                m_word = value.ToUpper();
                FillWordAction(this);
            }
        }

        public Action<CrosswordRowViewModel> FillWordAction { get; set; }
        
        public int AnswerPosition { get; private set; }

        public bool? IsCorrect
        {
            get { return m_isCorrect; }
            set
            {
                m_isCorrect = value;
                RaisePropertyChanged();
            }
        }

        public void UpdateWord(string word)
        {
            m_word = word.ToUpper();
            RaisePropertyChanged(() => Word);
        }

        private void CreateCells(int cellCount)
        {
            Cells = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < cellCount; i++)
            {
                Cells.Add(new CellViewModel());
            }
            
            Cells[AnswerPosition].IsPartOfAnswer = true;
        }

        public class CellViewModel : ViewModelBase
        {
            public bool IsPartOfAnswer { get; set; }
        }
    }
}