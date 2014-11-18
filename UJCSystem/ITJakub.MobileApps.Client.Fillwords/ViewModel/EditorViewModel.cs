using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords.DataService;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class EditorViewModel : ViewModelBase
    {
        private readonly FillwordsDataService m_dataService;
        private string m_selectedText;
        private Dictionary<int, OptionsViewModel> m_wordOptionsList;
        private OptionsViewModel m_selectedOption;
        private string m_newOption;
        private bool m_isEditorFlyoutOpen;

        public EditorViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            WordOptionsList = new Dictionary<int, OptionsViewModel>();
            AddNewOptionCommand = new RelayCommand(AddNewOption);
            SaveOptionsCommand = new RelayCommand(SaveOptions);
        }

        public RelayCommand AddNewOptionCommand { get; private set; }

        public RelayCommand SaveOptionsCommand { get; private set; }

        public string SelectedText
        {
            get { return m_selectedText; }
            set
            {
                m_selectedText = value;
                RaisePropertyChanged();
            }
        }

        public Dictionary<int, OptionsViewModel> WordOptionsList
        {
            get { return m_wordOptionsList; }
            set
            {
                m_wordOptionsList = value;
                RaisePropertyChanged();
            }
        }

        public OptionsViewModel SelectedOption
        {
            get { return m_selectedOption; }
            set
            {
                m_selectedOption = value;
                NewOption = string.Empty;
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

        public bool IsEditorFlyoutOpen
        {
            get { return m_isEditorFlyoutOpen; }
            set
            {
                m_isEditorFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        private void AddNewOption()
        {
            if (NewOption == string.Empty)
                return;

            var newOptionViewModel = new OptionViewModel
            {
                Word = NewOption,
            };
            newOptionViewModel.DeleteCommand = new RelayCommand(() => SelectedOption.List.Remove(newOptionViewModel));

            SelectedOption.List.Add(newOptionViewModel);

            NewOption = string.Empty;
        }

        private void SaveOptions()
        {
            var key = SelectedOption.WordPosition;

            if (SelectedOption.List.Count == 0)
                WordOptionsList.Remove(key);
            else
                WordOptionsList[key] = SelectedOption;

            IsEditorFlyoutOpen = false;
        }
    }
}
