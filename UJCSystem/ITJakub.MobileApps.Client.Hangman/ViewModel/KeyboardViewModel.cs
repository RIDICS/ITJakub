using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class KeyboardViewModel : ViewModelBase
    {
        private readonly List<KeyViewModel> m_basicKeys;

        public KeyboardViewModel()
        {
            m_basicKeys = new List<KeyViewModel>();

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                m_basicKeys.Add(new KeyViewModel(letter));
            }

            Keys = new ObservableCollection<KeyViewModel>(m_basicKeys);
        }

        public ObservableCollection<KeyViewModel> Keys { get; set; }

        public RelayCommand<char> ClickCommand { get; set; }

        public void DeactivateKey(char letter)
        {
            var keyViewModel = Keys.FirstOrDefault(key => key.Letter == letter);
            if (keyViewModel != null)
            {
                keyViewModel.IsEnabled = false;
            }
        }

        public void DeactivateKeys(IList<char> letters)
        {
            var keyViewModels = Keys.Where(key => letters.Contains(key.Letter));
            foreach (var keyViewModel in keyViewModels)
            {
                keyViewModel.IsEnabled = false;
            }
        }

        public void ReactivateAllKeys()
        {
            foreach (var keyViewModel in Keys)
            {
                keyViewModel.IsEnabled = true;
            }
        }

        public void SetSpecialLetters(IEnumerable<char> letters)
        {
            var keyList = m_basicKeys.ToList();
            
            foreach (var capitalLetter in letters.Select(Char.ToUpper).Where(letter => letter < 'A' || letter > 'Z').Distinct())
            {
                keyList.Add(new KeyViewModel(capitalLetter));
            }
            
            Keys = new ObservableCollection<KeyViewModel>(keyList.OrderBy(keyViewModel => keyViewModel.Letter.ToString(), new StringComparer()));
        }

        private class StringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }

    public class KeyViewModel : ViewModelBase
    {
        private bool m_isEnabled;

        public KeyViewModel(char letter)
        {
            IsEnabled = true;
            Letter = letter;
        }

        public char Letter { get; set; }

        public bool IsEnabled
        {
            get { return m_isEnabled; }
            set
            {
                m_isEnabled = value;
                RaisePropertyChanged();
            }
        }
    }
}