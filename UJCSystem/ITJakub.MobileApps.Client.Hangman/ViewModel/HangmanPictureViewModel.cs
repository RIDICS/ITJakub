using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanPictureViewModel : ViewModelBase
    {
        private bool m_construction1Visible;
        private bool m_construction2Visible;
        private bool m_construction3Visible;
        private bool m_ropeVisible;
        private bool m_baseVisible;
        private bool m_headVisible;
        private bool m_bodyVisible;
        private bool m_leftArmVisible;
        private bool m_rightArmVisible;
        private bool m_leftLegVisible;
        private bool m_rightLegVisible;
        private bool m_leftEyeVisible;
        private bool m_rightEyeVisible;
        private bool m_mouthVisible;
        private int m_lives;
        private bool m_noseVisible;
        private int m_currentHangmanPicture;

        public int Lives
        {
            set
            {
                m_lives = value;
                LivesUpdate(m_lives);
            }
        }

        private void LivesUpdate(int lives)
        {
            MouthVisible = lives == 0;
            NoseVisible = lives < 2;
            RightEyeVisible = lives < 3;
            LeftEyeVisible = lives < 4;
            RightLegVisible = lives < 5;
            LeftLegVisible = lives < 6;
            RightArmVisible = lives < 7;
            LeftArmVisible = lives < 8;
            BodyVisible = lives < 9;
            HeadVisible = lives < 10;
            RopeVisible = lives < 11;
            Construction3Visible = lives < 12;
            Construction2Visible = lives < 13;
            Construction1Visible = lives < 14;
            BaseVisible = lives < 15;
        }

        public int CurrentHangmanPicture
        {
            get { return m_currentHangmanPicture; }
            set
            {
                m_currentHangmanPicture = value;
                RaisePropertyChanged();
            }
        }

        public bool Construction1Visible
        {
            get { return m_construction1Visible; }
            set
            {
                m_construction1Visible = value;
                RaisePropertyChanged();
            }
        }

        public bool Construction2Visible
        {
            get { return m_construction2Visible; }
            set
            {
                m_construction2Visible = value;
                RaisePropertyChanged();
            }
        }

        public bool Construction3Visible
        {
            get { return m_construction3Visible; }
            set
            {
                m_construction3Visible = value;
                RaisePropertyChanged();
            }
        }

        public bool RopeVisible
        {
            get { return m_ropeVisible; }
            set
            {
                m_ropeVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool BaseVisible
        {
            get { return m_baseVisible; }
            set
            {
                m_baseVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool HeadVisible
        {
            get { return m_headVisible; }
            set
            {
                m_headVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool BodyVisible
        {
            get { return m_bodyVisible; }
            set
            {
                m_bodyVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool LeftArmVisible
        {
            get { return m_leftArmVisible; }
            set
            {
                m_leftArmVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool RightArmVisible
        {
            get { return m_rightArmVisible; }
            set
            {
                m_rightArmVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool LeftLegVisible
        {
            get { return m_leftLegVisible; }
            set
            {
                m_leftLegVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool RightLegVisible
        {
            get { return m_rightLegVisible; }
            set
            {
                m_rightLegVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool LeftEyeVisible
        {
            get { return m_leftEyeVisible; }
            set
            {
                m_leftEyeVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool RightEyeVisible
        {
            get { return m_rightEyeVisible; }
            set
            {
                m_rightEyeVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool NoseVisible
        {
            get { return m_noseVisible; }
            set
            {
                m_noseVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool MouthVisible
        {
            get { return m_mouthVisible; }
            set
            {
                m_mouthVisible = value;
                RaisePropertyChanged();
            }
        }
    }
}
