using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Hangman
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HangmanViewModel : ApplicationBaseViewModel
    {
        private string m_test;

        /// <summary>
        /// Initializes a new instance of the HangmanViewModel class.
        /// </summary>
        public HangmanViewModel()
        {
            Test = "TEST test test";
        }

        public string Test
        {
            get { return m_test; }
            set
            {
                m_test = value;
                RaisePropertyChanged();
            }
        }

        public override void InitializeCommunication()
        {
            //TODO init communication and start timers
        }

        public override void StopTimers()
        {
            //TODO stop timers
        }
    }
}