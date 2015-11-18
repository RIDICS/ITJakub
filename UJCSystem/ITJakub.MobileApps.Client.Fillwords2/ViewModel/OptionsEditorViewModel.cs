using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class OptionsEditorViewModel : ViewModelBase
    {
        private readonly Dictionary<int, WordOptionsViewModel> m_wordOptionsList;
        private bool m_showOptionExistsInfo;
        private string m_newOption;
        private WordOptionsViewModel m_selectedOption;
        private string m_selectedText;
        private bool m_setSelectedTextHighlighted;
        private int m_selectionStart;
        private bool m_isSelected;
        private bool m_isReset;

        public OptionsEditorViewModel()
        {
            m_wordOptionsList = new Dictionary<int, WordOptionsViewModel>();

            //AddNewOptionCommand = new RelayCommand(AddNewOption);
            //DeleteCommand = new RelayCommand<LetterOptionViewModel>(DeleteOption);
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
        }

        public Dictionary<int, WordOptionsViewModel> WordOptionsList
        {
            get
            {
                UpdateWordOptionsList();
                return m_wordOptionsList;
            }
        }

        public bool ShowOptionExistsInfo
        {
            get { return m_showOptionExistsInfo; }
            set
            {
                m_showOptionExistsInfo = value;
                RaisePropertyChanged();
            }
        }

        public string NewOption
        {
            get { return m_newOption; }
            set
            {
                m_newOption = value;
                RaisePropertyChanged();
            }
        }

        public WordOptionsViewModel SelectedOption
        {
            get { return m_selectedOption; }
            private set
            {
                m_selectedOption = value;
                NewOption = string.Empty;
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
        
        public bool SetSelectedTextHighlighted
        {
            get { return m_setSelectedTextHighlighted; }
            set
            {
                m_setSelectedTextHighlighted = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool IsReset
        {
            get { return m_isReset; }
            set
            {
                m_isReset = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SelectionChangedCommand { get; private set; }
        
        private void UpdateWordOptionsList()
        {
            if (SelectedOption == null)
                return;
            
            if (SelectedOption.Options.Count == 0)
            {
                m_wordOptionsList.Remove(SelectedOption.WordPosition);
            }
            else if (!m_wordOptionsList.ContainsKey(SelectedOption.WordPosition))
            {
                m_wordOptionsList.Add(SelectedOption.WordPosition, SelectedOption);
                IsReset = false;
            }
        }

        private void SelectionChanged()
        {
            UpdateWordOptionsList();
            IsSelected = !string.IsNullOrEmpty(SelectedText);

            SelectedOption = m_wordOptionsList.ContainsKey(SelectionStart)
                ? m_wordOptionsList[SelectionStart]
                : new WordOptionsViewModel(SelectedText, SelectionStart);
            
            SelectedOption.HighlightChangeCallback = isHighlight =>
            {
                SetSelectedTextHighlighted = isHighlight;
            };
            SelectedOption.RecoverHighlight();
        }

        public void Reset()
        {
            m_wordOptionsList.Clear();
            SelectedOption = null;
            IsReset = true;
            IsSelected = false;
        }
    }
}