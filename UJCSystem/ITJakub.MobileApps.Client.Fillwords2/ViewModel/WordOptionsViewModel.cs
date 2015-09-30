using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class WordOptionsViewModel : ViewModelBase
    {
        //private AnswerState m_answerState = AnswerState.NoAnswer;
        //private string m_selectedAnswer;
        //private string m_correctAnswer;
        private ObservableCollection<LetterOptionViewModel> m_options;
        private int m_wordPosition;
        private LetterOptionViewModel m_currentOption;
        private int m_selectionStart;
        private int m_selectionEnd;
        private string m_selectedText;
        private string m_selectedWord;
        private bool m_isSelectedTextHighlighted;
        private bool m_isSelected;

        public WordOptionsViewModel(string selectedWord, int selectionStart)
        {
            m_selectedWord = selectedWord;
            WordPosition = selectionStart;
            
            Options = new ObservableCollection<LetterOptionViewModel>();
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
        }

        private void SelectionChanged()
        {
            if (SelectionStart == SelectionEnd)
            {
                CurrentOption = null;
                return;
            }

            if (CurrentOption == null)
            {
                var currentOptions = Options.FirstOrDefault(x => x.StartPosition == SelectionStart);
                if (currentOptions == null)
                {
                    IsSelectedTextHighlighted = false;
                    currentOptions = new LetterOptionViewModel
                    {
                        StartPosition = SelectionStart,
                        Length = SelectionEnd - SelectionStart,
                        Letters = SelectedText
                    };
                    currentOptions.AnswerTypeViewModel.AnswerChangedCallback = AnswerTypeChanged;
                }
                else
                {
                    IsSelectedTextHighlighted = true;
                }

                CurrentOption = currentOptions;
                return;
            };

            switch (CurrentOption.AnswerTypeViewModel.AnswerType)
            {
                case AnswerType.NoAnswer:
                    Options.Remove(CurrentOption);
                    break;
                case AnswerType.Fill:
                    Options.Add(CurrentOption);
                    break;
                case AnswerType.Selection:
                    break;
            }
            
        }

        private void AnswerTypeChanged()
        {
            if (CurrentOption.AnswerTypeViewModel.AnswerType == AnswerType.NoAnswer)
            {
                IsSelectedTextHighlighted = false;
            }
            else
            {
                IsSelectedTextHighlighted = true;
            }
        }


        //public Action<WordOptionsViewModel> AnswerChangedCallback { get; set; } 

        public string SelectedWord
        {
            get { return m_selectedWord; }
        }

        public int WordPosition
        {
            get { return m_wordPosition; }
            set
            {
                m_wordPosition = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LetterOptionViewModel> Options
        {
            get { return m_options; }
            set
            {
                m_options = value;
                RaisePropertyChanged();
            }
        }

        public LetterOptionViewModel CurrentOption
        {
            get { return m_currentOption; }
            set
            {
                m_currentOption = value;
                RaisePropertyChanged();
            }
        }

        public int SelectionStart
        {
            get { return m_selectionStart; }
            set
            {
                m_selectionStart = value;
                RaisePropertyChanged();
            }
        }

        public int SelectionEnd
        {
            get { return m_selectionEnd; }
            set
            {
                m_selectionEnd = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedText
        {
            get { return m_selectedText; }
            set
            {
                m_selectedText = value;
                RaisePropertyChanged();
                IsSelected = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsSelectedTextHighlighted
        {
            get { return m_isSelectedTextHighlighted; }
            set
            {
                m_isSelectedTextHighlighted = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand { get; private set; }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                RaisePropertyChanged();
            }
        }


        //public string CorrectAnswer
        //{
        //    get { return m_correctAnswer; }
        //    set
        //    {
        //        m_correctAnswer = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //public string SelectedAnswer
        //{
        //    get { return m_selectedAnswer; }
        //    set
        //    {
        //        m_selectedAnswer = value;
        //        RaisePropertyChanged();

        //        if (AnswerChangedCallback != null)
        //            AnswerChangedCallback(this);
        //    }
        //}

        //public void UpdateSelectedAnswer(string newAnswer)
        //{
        //    m_selectedAnswer = newAnswer;
        //    RaisePropertyChanged(() => SelectedAnswer);
        //}

        //public AnswerState AnswerState
        //{
        //    get { return m_answerState; }
        //    set
        //    {
        //        m_answerState = value;
        //        RaisePropertyChanged();
        //    }
        //}
    }

    public class LetterOptionViewModel : ViewModelBase
    {
        private string m_letters;

        public LetterOptionViewModel()
        {
            AnswerTypeViewModel = new AnswerTypeViewModel();
        }

        public AnswerTypeViewModel AnswerTypeViewModel { get; set; }

        public string Letters
        {
            get { return m_letters; }
            set
            {
                m_letters = value;
                RaisePropertyChanged();
            }
        }

        public int StartPosition { get; set; }

        public int Length { get; set; }
    }
}