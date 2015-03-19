using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsEditorViewModel : ViewModelBase
    {
        private readonly Dictionary<int, OptionsViewModel> m_wordOptionsList;
        private bool m_showOptionExistsInfo;
        private string m_newOption;
        private OptionsViewModel m_selectedOption;
        private string m_selectedText;
        private bool m_setSelectedTextHighlighted;
        private int m_selectionStart;
        private bool m_isSelected;
        private bool m_isReset;

        public OptionsEditorViewModel()
        {
            m_wordOptionsList = new Dictionary<int, OptionsViewModel>();

            AddNewOptionCommand = new RelayCommand(AddNewOption);
            DeleteCommand = new RelayCommand<OptionViewModel>(DeleteOption);
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
        }

        public Dictionary<int, OptionsViewModel> WordOptionsList
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

        public OptionsViewModel SelectedOption
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

        public RelayCommand AddNewOptionCommand { get; private set; }

        public RelayCommand<OptionViewModel> DeleteCommand { get; private set; }

        public RelayCommand SelectionChangedCommand { get; private set; }
        

        private void AddNewOption()
        {
            if (NewOption == string.Empty)
                return;

            if (SelectedOption.List == null)
            {
                SelectedOption.List = new ObservableCollection<OptionViewModel>();
                SetSelectedTextHighlighted = true;
            }

            if (SelectedOption.List.Any(model => model.Word == NewOption))
            {
                ShowOptionExistsInfo = true;
            }
            else
            {
                var newOptionViewModel = new OptionViewModel { Word = NewOption };

                SelectedOption.List.Add(newOptionViewModel);
                ShowOptionExistsInfo = false;
                NewOption = string.Empty;
            }
        }

        private void DeleteOption(OptionViewModel option)
        {
            SelectedOption.List.Remove(option);

            if (SelectedOption.List.Count == 0)
            {
                SelectedOption.List = null;
                SetSelectedTextHighlighted = false;
            }
        }

        private void UpdateWordOptionsList()
        {
            if (SelectedOption == null)
                return;
            
            if (SelectedOption.List == null)
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
                : new OptionsViewModel {CorrectAnswer = SelectedText, WordPosition = SelectionStart};
        }

        public void Reset()
        {
            m_wordOptionsList.Clear();
            IsReset = true;
        }
    }
}