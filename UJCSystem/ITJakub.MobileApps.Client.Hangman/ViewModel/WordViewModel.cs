using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class WordViewModel : ViewModelBase
    {
        private List<char[]> m_allLetters;
        private string m_word;

        public WordViewModel()
        {
            SpreadWord(string.Empty);
        }

        private void SpreadWord(string word)
        {
            var words = word.Split(' ');
            var letters = new List<char[]>(words.Length);
            foreach (var w in words)
            {
                letters.Add(w.ToCharArray());
            }

            AllLetters = letters;
        }

        public string Word
        {
            set
            {
                m_word = value;
                RaisePropertyChanged();
                SpreadWord(m_word);
            }
        }

        public List<char[]> AllLetters
        {
            get { return m_allLetters; }
            private set
            {
                m_allLetters = value;
                RaisePropertyChanged();
            }
        }
    }
}
