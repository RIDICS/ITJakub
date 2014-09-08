using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private int m_lives;
        private bool m_win;
        private bool m_opponentProgressVisible;
        private bool m_guessHistoryVisible;
        private int m_guessedLetterCount;
        private WordViewModel m_wordViewModel;

        /// <summary>
        /// Initializes a new instance of the HangmanViewModel class.
        /// </summary>
        /// <param name="dataService"></param>
        public HangmanViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
            GuessHistory = new ObservableCollection<GuessViewModel>();
            OpponentProgress = new ObservableCollection<ProgressInfoViewModel>();
            GuessCommand = new RelayCommand(Guess);
            HangmanPictureViewModel = new HangmanPictureViewModel();
            WordViewModel = new WordViewModel();
        }

        public ObservableCollection<GuessViewModel> GuessHistory { get; set; }

        public ObservableCollection<ProgressInfoViewModel> OpponentProgress { get; set; }

        public HangmanPictureViewModel HangmanPictureViewModel { get; set; }

        public RelayCommand GuessCommand { get; private set; }

        public string Letter
        {
            get { return m_letter; }
            set
            {
                m_letter = value;
                RaisePropertyChanged();
            }
        }

        public bool GameOver
        {
            get { return m_gameOver; }
            set
            {
                m_gameOver = value;
                RaisePropertyChanged();
            }
        }

        public int Lives
        {
            get { return m_lives; }
            set
            {
                m_lives = value;
                RaisePropertyChanged();
                HangmanPictureViewModel.Lives = m_lives;
            }
        }

        public int GuessedLetterCount
        {
            get { return m_guessedLetterCount; }
            set
            {
                m_guessedLetterCount = value;
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

        public bool OpponentProgressVisible
        {
            get { return m_opponentProgressVisible; }
            set
            {
                m_opponentProgressVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool GuessHistoryVisible
        {
            get { return m_guessHistoryVisible; }
            set
            {
                m_guessHistoryVisible = value;
                RaisePropertyChanged();
            }
        }

        public WordViewModel WordViewModel
        {
            get { return m_wordViewModel; }
            set
            {
                m_wordViewModel = value;
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
                SetDataLoaded();
            });

            m_dataService.StartPollingProgress((progressInfo, exception) =>
            {
                if (exception != null)
                    return;

                ProcessOpponentProgress(progressInfo);
            });
        }

        public override void SetTask(string data)
        {
            // TODO get correct application mode from method parameter
            m_dataService.SetTaskAndGetWord(data, GuessManager.VersusMode, (taskSettings, taskInfo) =>
            {
                GuessHistoryVisible = taskSettings.GuessHistoryVisible;
                OpponentProgressVisible = taskSettings.OpponentProgressVisible;
                ProcessTaskInfo(taskInfo);
            });
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }

        private void ProcessTaskInfo(TaskInfoViewModel taskInfo)
        {
            WordViewModel.Word = taskInfo.Word;
            GameOver = taskInfo.Lives == 0;
            Lives = taskInfo.Lives;
            GuessedLetterCount = taskInfo.GuessedLetterCount;

            if (taskInfo.Win)
            {
                // TODO show correct message
                Win = true;
                GameOver = true;
            }

            // TODO if is new word, clear guess history
        }

        private void ProcessOpponentProgress(IEnumerable<ProgressInfoViewModel> progressUpdate)
        {
            foreach (var progressInfo in progressUpdate)
            {
                if (progressInfo.UserInfo.IsMe)
                    continue;

                var viewModel = OpponentProgress.SingleOrDefault(model => model.UserInfo.Id == progressInfo.UserInfo.Id);

                if (viewModel != null)
                {
                    viewModel.Lives = progressInfo.Lives;
                    viewModel.LetterCount = progressInfo.LetterCount;
                }
                else
                {
                    OpponentProgress.Add(progressInfo);
                }
            }
        }
        
        private void Guess()
        {
            if (Letter == string.Empty || Letter == " " || GameOver)
                return;

            m_dataService.GuessLetter(Letter[0], (taskInfo, exception) =>
            {
                if (exception != null)
                    return;

                ProcessTaskInfo(taskInfo);
            });

            Letter = string.Empty;
        }
    }
}