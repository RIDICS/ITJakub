using System;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class ProgressInfoViewModel : ViewModelBase
    {
        private int m_livesRemain;
        private int m_letterCount;
        private bool m_win;
        private DateTime m_time;
        private int m_guessedWordCount;
        private int m_hangmanCount;
        private int m_hangmanPicture;

        public ProgressInfoViewModel()
        {
            PictureViewModel = new HangmanPictureViewModel();
        }

        public int GuessedWordCount
        {
            get { return m_guessedWordCount; }
            set
            {
                m_guessedWordCount = value;
                RaisePropertyChanged();
            }
        }

        public int LivesRemain
        {
            get { return m_livesRemain; }
            set
            {
                m_livesRemain = value;
                PictureViewModel.Lives = m_livesRemain;
                RaisePropertyChanged();
            }
        }

        public int HangmanCount
        {
            get { return m_hangmanCount; }
            set
            {
                m_hangmanCount = value;
                RaisePropertyChanged();
            }
        }

        public int LetterCount
        {
            get { return m_letterCount; }
            set
            {
                m_letterCount = value;
                RaisePropertyChanged();
            }
        }

        public bool Win
        {
            get { return m_win; }
            set
            {
                m_win = value;
                RaisePropertyChanged();
            }
        }
        
        public DateTime Time
        {
            get { return m_time; }
            set
            {
                m_time = value;
                RaisePropertyChanged();
            }
        }

        public int HangmanPicture
        {
            get { return m_hangmanPicture; }
            set
            {
                m_hangmanPicture = value;
                RaisePropertyChanged();
                PictureViewModel.CurrentHangmanPicture = m_hangmanPicture;
            }
        }

        public UserInfo UserInfo { get; set; }

        public HangmanPictureViewModel PictureViewModel { get; private set; }

        public DateTime FirstUpdateTime { get; set; }
    }
}