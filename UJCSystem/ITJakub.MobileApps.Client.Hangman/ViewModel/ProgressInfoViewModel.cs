using System;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class ProgressInfoViewModel : ViewModelBase
    {
        private int m_lives;
        private int m_letterCount;
        private bool m_win;
        private DateTime m_time;

        public ProgressInfoViewModel()
        {
            PictureViewModel = new HangmanPictureViewModel();
        }

        public int Lives
        {
            get { return m_lives; }
            set
            {
                m_lives = value;
                PictureViewModel.Lives = m_lives;
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

        public UserInfo UserInfo { get; set; }

        public HangmanPictureViewModel PictureViewModel { get; private set; }

        public DateTime FirstUpdateTime { get; set; }
    }
}