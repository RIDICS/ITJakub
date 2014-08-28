using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HangmanViewModel : ApplicationBaseViewModel
    {
        private readonly IHangmanDataService m_dataService;
        private string m_wordToGuess;
        private string m_letter;
        private bool m_gameOver;

        /// <summary>
        /// Initializes a new instance of the HangmanViewModel class.
        /// </summary>
        /// <param name="dataService"></param>
        public HangmanViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
            GuessHistory = new ObservableCollection<GuessViewModel>();
            GuessCommand = new RelayCommand(Guess);
        }

        public ObservableCollection<GuessViewModel> GuessHistory { get; set; }

        public string Letter
        {
            get { return m_letter; }
            set
            {
                m_letter = value;
                RaisePropertyChanged();
            }
        }

        public string WordToGuess
        {
            get { return m_wordToGuess; }
            set
            {
                m_wordToGuess = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GuessCommand { get; private set; }

        public bool GameOver
        {
            get { return m_gameOver; }
            set
            {
                m_gameOver = value;
                RaisePropertyChanged();
            }
        }

        public override void InitializeCommunication()
        {
            m_dataService.StartPollingLetters((guesses, taskInfo, exception) =>
            {
                if (exception != null)
                    return;

                foreach (var guessViewModel in guesses)
                {
                    GuessHistory.Add(guessViewModel);
                }

                ProcessTaskInfo(taskInfo);
            });
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetWord(data, ProcessTaskInfo);
        }

        public override void StopCommunication()
        {
            m_dataService.StopPollingLetters();
        }

        private void ProcessTaskInfo(TaskInfoViewModel taskInfo)
        {
            WordToGuess = taskInfo.Word;
            GameOver = taskInfo.Lives == 0;
            //TODO set lives to Hangman image
        }

        private void Guess()
        {
            if (Letter == string.Empty || Letter == " ")
                return;

            //TODO show correct letter immediately
            m_dataService.GuessLetter(Letter[0], exception =>
            {
                if (exception != null)
                    return;
            });

            Letter = string.Empty;
        }
    }
}