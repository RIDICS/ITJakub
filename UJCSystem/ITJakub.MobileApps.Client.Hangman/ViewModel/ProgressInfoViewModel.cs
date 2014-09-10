using System;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class ProgressInfoViewModel : ViewModelBase
    {
        private int m_lives;
        private int m_letterCount;

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

        public AuthorInfo UserInfo { get; set; }

        public HangmanPictureViewModel PictureViewModel { get; private set; }

        public DateTime Time { get; set; }

        public DateTime FirstUpdateTime { get; set; }
    }
}