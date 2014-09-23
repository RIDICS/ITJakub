using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Crosswords.DataService;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class SimpleCrosswordsViewModel : ViewModelBase
    {
        private readonly ICrosswordsDataService m_dataService;
        private ObservableCollection<CrosswordRowViewModel> m_crossword;
        private bool m_firstUpdate;

        public SimpleCrosswordsViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;
            m_firstUpdate = true;
            Crossword = new ObservableCollection<CrosswordRowViewModel>();
            OpponentProgress = new ObservableCollection<ProgressViewModel>();
        }

        public ObservableCollection<CrosswordRowViewModel> Crossword
        {
            get { return m_crossword; }
            set
            {
                m_crossword = value;
                RaisePropertyChanged();
                SetSubmitCallbacks(m_crossword);
            }
        }

        public ObservableCollection<ProgressViewModel> OpponentProgress { get; set; }

        private void SetSubmitCallbacks(IEnumerable<CrosswordRowViewModel> crosswordRows)
        {
            foreach (CrosswordRowViewModel rowViewModel in crosswordRows)
            {
                rowViewModel.FillWordAction = SubmitAction;
            }
        }

        private void SubmitAction(CrosswordRowViewModel rowViewModel)
        {
            m_dataService.FillWord(rowViewModel.RowIndex, rowViewModel.Word, (isCorrect, exception) =>
            {
                if (exception != null)
                    return;

                rowViewModel.IsCorrect = isCorrect;
            });
        }

        public void UpdateProgress(IEnumerable<ProgressUpdateViewModel> list)
        {
            foreach (var progressUpdate in list)
            {
                if (progressUpdate.UserInfo.IsMe)
                {
                    // Load my progress history (only first load)
                    if (!m_firstUpdate)
                        continue;

                    var rowViewModel = Crossword[progressUpdate.RowIndex];
                    rowViewModel.UpdateWord(progressUpdate.FilledWord);
                    rowViewModel.IsCorrect = progressUpdate.IsCorrect;
                }
                else
                {
                    // Load opponent progress
                    var viewModel = OpponentProgress.SingleOrDefault(model => model.UserInfo.Id == progressUpdate.UserInfo.Id);
                    if (viewModel == null)
                    {
                        var rowProgressViewModels = Crossword.Select(model => model.Cells != null
                            ? new RowProgressViewModel(model.Cells.Count, model.StartPosition, model.AnswerPosition)
                            : new RowProgressViewModel());
                        
                        viewModel = new ProgressViewModel
                        {
                            UserInfo = progressUpdate.UserInfo,
                            Rows = new ObservableCollection<RowProgressViewModel>(rowProgressViewModels)
                        };

                        OpponentProgress.Add(viewModel);
                    }
                    var rowViewModel = viewModel.Rows[progressUpdate.RowIndex];
                    rowViewModel.FilledLength = progressUpdate.FilledWord.Length;
                    rowViewModel.IsCorrect = progressUpdate.IsCorrect;
                }
            }
            m_firstUpdate = false;
        }
    }
}