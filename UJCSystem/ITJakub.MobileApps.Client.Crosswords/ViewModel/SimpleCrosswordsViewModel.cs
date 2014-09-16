using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Crosswords.DataService;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class SimpleCrosswordsViewModel : ViewModelBase
    {
        private readonly ICrosswordsDataService m_dataService;
        private ObservableCollection<CrosswordRowViewModel> m_crossword;

        public SimpleCrosswordsViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;
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

                if (isCorrect)
                {
                    //TODO better way to display correct answer
                    new MessageDialog("vyplneno spravne").ShowAsync();
                }
            });
        }
    }
}