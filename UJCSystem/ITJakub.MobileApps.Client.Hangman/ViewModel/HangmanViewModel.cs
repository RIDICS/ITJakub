using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanViewModel : ApplicationBaseViewModel
    {
        private readonly IHangmanDataService m_dataService;
        private int m_lives;
        private bool m_opponentProgressVisible;
        private int m_guessedLetterCount;
        private bool m_isAppStopped;
        private string m_currentHint;
        private int m_guessedWordCount;

        public HangmanViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
            OpponentProgress = new ObservableCollection<ProgressInfoViewModel>();
            HangmanPictureViewModel = new HangmanPictureViewModel();
            WordViewModel = new WordViewModel();
            KeyboardViewModel = new KeyboardViewModel();
            GameOverViewModel = new GameOverViewModel();

            KeyboardViewModel.ClickCommand = new RelayCommand<char>(Guess);
        }
        
        public HangmanPictureViewModel HangmanPictureViewModel { get; set; }

        public WordViewModel WordViewModel { get; set; }

        public KeyboardViewModel KeyboardViewModel { get; set; }

        public GameOverViewModel GameOverViewModel { get; set; }

        public ObservableCollection<ProgressInfoViewModel> OpponentProgress { get; set; }

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

        public int GuessedWordCount
        {
            get { return m_guessedWordCount; }
            set
            {
                m_guessedWordCount = value;
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

        public string CurrentHint
        {
            get { return m_currentHint; }
            set
            {
                m_currentHint = value;
                RaisePropertyChanged();
            }
        }

        public override void InitializeCommunication()
        {
            //TODO remove guesses and rename to GetTaskInfo
            m_dataService.StartPollingLetters((guesses, taskInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError(GoBack);
                    return;
                }

                ProcessTaskInfo(taskInfo);
                SetDataLoaded();
            });

            m_dataService.StartPollingProgress((progressInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                ProcessOpponentProgress(progressInfo);
            });
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetConfiguration(data, (taskSettings, taskInfo) =>
            {
                if (taskSettings.SpecialLetters != null)
                    KeyboardViewModel.SetSpecialLetters(taskSettings.SpecialLetters);

                OpponentProgressVisible = taskSettings.OpponentProgressVisible;
                ProcessTaskInfo(taskInfo);
            });
        }

        public override void EvaluateAndShowResults()
        {
            m_isAppStopped = true;
            GameOverViewModel.Loss = true;
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get { return new ActionViewModel[0]; }
        }
        
        private void ProcessTaskInfo(TaskProgressInfoViewModel taskProgressInfo)
        {
            WordViewModel.Word = taskProgressInfo.Word;
            CurrentHint = taskProgressInfo.Hint;
            Lives = taskProgressInfo.Lives;
            GuessedLetterCount = taskProgressInfo.GuessedLetterCount;
            GuessedWordCount = taskProgressInfo.GuessedWordCount;

            if (taskProgressInfo.Win)
            {
                GameOverViewModel.Win = true;
            }
            else
            {
                GameOverViewModel.Loss = m_isAppStopped || taskProgressInfo.Lives == 0;
            }

            if (taskProgressInfo.IsNewWord)
            {
                KeyboardViewModel.ReactivateAllKeys();
            }
        }
        
        private void ProcessOpponentProgress(ICollection<ProgressInfoViewModel> progressUpdate)
        {
            foreach (var progressInfo in progressUpdate)
            {
                if (progressInfo.UserInfo.IsMe)
                {
                    GameOverViewModel.UpdateMyProgress(progressInfo);
                    continue;
                }

                var viewModel = OpponentProgress.SingleOrDefault(model => model.UserInfo.Id == progressInfo.UserInfo.Id);

                if (viewModel != null)
                {
                    viewModel.HangmanCount = progressInfo.HangmanCount;
                    viewModel.LivesRemain = progressInfo.LivesRemain;
                    viewModel.GuessedWordCount = progressInfo.GuessedWordCount;
                    viewModel.LetterCount = progressInfo.LetterCount;
                    viewModel.Win = progressInfo.Win;
                    viewModel.Time = progressInfo.Time;
                }
                else
                {
                    progressInfo.FirstUpdateTime = progressInfo.Time;
                    OpponentProgress.Add(progressInfo);
                    GameOverViewModel.AddPlayerViewModel(progressInfo);
                }
            }
            if (progressUpdate.Count > 0)
            {
                GameOverViewModel.UpdatePlayerPositions();
            }
        }

        private void Guess(char letter)
        {
            if (GameOverViewModel.GameOver)
                return;

            KeyboardViewModel.DeactivateKey(letter);
            m_dataService.GuessLetter(letter, (taskInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowError("Nepodařilo se odeslat Vámi hádaná písmena. Zkontrolujte připojení k internetu. Aplikace bude ukončena.", "Nelze kontaktovat server", GoBack);
                    return;
                }

                ProcessTaskInfo(taskInfo);
            });
        }
    }
}