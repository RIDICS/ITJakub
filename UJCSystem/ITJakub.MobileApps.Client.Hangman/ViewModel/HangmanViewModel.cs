using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.ViewModel
{
    public class HangmanViewModel : ApplicationBaseViewModel
    {
        private readonly IHangmanDataService m_dataService;
        private int m_lives;
        private bool m_opponentProgressVisible;
        private bool m_guessHistoryVisible;
        private int m_guessedLetterCount;
        private bool m_isAppStopped;

        public HangmanViewModel(IHangmanDataService dataService)
        {
            m_dataService = dataService;
            GuessHistory = new ObservableCollection<GuessViewModel>();
            OpponentProgress = new ObservableCollection<ProgressInfoViewModel>();
            HangmanPictureViewModel = new HangmanPictureViewModel();
            WordViewModel = new WordViewModel();
            KeyboardViewModel = new KeyboardViewModel();
            GameOverViewModel = new GameOverViewModel();

            KeyboardViewModel.ClickCommand = new RelayCommand<char>(Guess);
        }

        public ObservableCollection<GuessViewModel> GuessHistory { get; set; }

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

        public override void InitializeCommunication()
        {
            m_dataService.StartPollingLetters((guesses, taskInfo, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionError(GoBack);
                    return;
                }

                ProcessNewLetters(guesses);
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
            // TODO get correct application mode from method parameter
            m_dataService.SetTaskAndGetConfiguration(data, GuessManager.VersusMode, (taskSettings, taskInfo) =>
            {
                if (taskSettings.SpecialLetters != null)
                    KeyboardViewModel.SetSpecialLetters(taskSettings.SpecialLetters);

                GuessHistoryVisible = taskSettings.GuessHistoryVisible;
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

        private void ProcessNewLetters(IEnumerable<GuessViewModel> guesses)
        {
            var wordIndex = 0;
            var lastViewModel = GuessHistory.LastOrDefault();
            if (lastViewModel != null)
                wordIndex = lastViewModel.WordOrder;

            foreach (var guessViewModel in guesses)
            {
                if (guessViewModel.WordOrder > wordIndex)
                {
                    GuessHistory.Add(new GuessViewModel
                    {
                        Letter = '-',
                        WordOrder = wordIndex,
                        Author = new UserInfo()
                    });
                    wordIndex = guessViewModel.WordOrder;
                }

                GuessHistory.Add(guessViewModel);
                KeyboardViewModel.DeactivateKey(guessViewModel.Letter);
            }
        }

        private void ProcessTaskInfo(TaskInfoViewModel taskInfo)
        {
            WordViewModel.Word = taskInfo.Word;
            Lives = taskInfo.Lives;
            GuessedLetterCount = taskInfo.GuessedLetterCount;

            if (taskInfo.Win)
            {
                GameOverViewModel.Win = true;
            }
            else
            {
                GameOverViewModel.Loss = m_isAppStopped || taskInfo.Lives == 0;
            }

            if (taskInfo.IsNewWord)
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
                    viewModel.Lives = progressInfo.Lives;
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