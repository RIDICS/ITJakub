using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class OptionsEditorViewModel : ViewModelBase
    {
        private readonly Dictionary<int, OptionsViewModel> m_wordOptionsList;
        private readonly Action m_closeFlyoutAction;
        private bool m_showOptionExistsInfo;
        private string m_newOption;
        private OptionsViewModel m_selectedOption;
        private string m_selectedText;
        private bool m_setSelectedTextHighlighted;

        public OptionsEditorViewModel(Dictionary<int, OptionsViewModel> wordOptionsList, Action closeFlyoutAction)
        {
            m_wordOptionsList = wordOptionsList;
            m_closeFlyoutAction = closeFlyoutAction;

            AddNewOptionCommand = new RelayCommand(AddNewOption);
            SaveOptionsCommand = new RelayCommand(SaveOptions);
            DeleteCommand = new RelayCommand<OptionViewModel>(DeleteOption);
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
            set
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
        
        public bool SetSelectedTextHighlighted
        {
            get { return m_setSelectedTextHighlighted; }
            set
            {
                m_setSelectedTextHighlighted = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand AddNewOptionCommand { get; private set; }

        public RelayCommand SaveOptionsCommand { get; private set; }

        public RelayCommand<OptionViewModel> DeleteCommand { get; private set; }
        
        private void AddNewOption()
        {
            if (NewOption == string.Empty)
                return;

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
        }

        private void SaveOptions()
        {
            var key = SelectedOption.WordPosition;

            if (SelectedOption.List.Count == 0)
            {
                m_wordOptionsList.Remove(key);
                SetSelectedTextHighlighted = false;
            }
            else
            {
                m_wordOptionsList[key] = SelectedOption;
                SetSelectedTextHighlighted = true;
            }

            m_closeFlyoutAction();
        }
    }
}