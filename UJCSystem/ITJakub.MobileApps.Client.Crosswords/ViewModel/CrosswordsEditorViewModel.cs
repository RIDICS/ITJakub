using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsEditorViewModel : EditorBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;
        private bool m_isSaveFlyoutOpen;
        private bool m_errorTaskNameEmpty;
        private bool m_errorAnswerListEmpty;
        private bool m_errorSomeAnswerEmpty;
        private int m_answerColumn;

        public CrosswordsEditorViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;

            AnswerList = new ObservableCollection<ItemViewModel> {new ItemViewModel(), new ItemViewModel()};

            SaveTaskCommand = new RelayCommand(SaveTask);
            AddAnswerCommand = new RelayCommand(AddAnswer);
            DeleteAnswerCommand = new RelayCommand<ItemViewModel>(DeleteAnswer);

            ShiftLeftCommand = new RelayCommand(() =>
            {
                if (AnswerColumn > 0)
                    AnswerColumn--;
            });
            ShiftRightCommand = new RelayCommand(() =>
            {
                AnswerColumn++;
            });
        }

        public RelayCommand SaveTaskCommand { get; private set; }

        public RelayCommand AddAnswerCommand { get; private set; }

        public RelayCommand<ItemViewModel> DeleteAnswerCommand { get; private set; }

        public RelayCommand ShiftLeftCommand { get; private set; }

        public RelayCommand ShiftRightCommand { get; private set; }

        public ObservableCollection<ItemViewModel> AnswerList { get; set; }

        public string TaskName { get; set; }

        public bool IsSaveFlyoutOpen
        {
            get { return m_isSaveFlyoutOpen; }
            set
            {
                m_isSaveFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorTaskNameEmpty
        {
            get { return m_errorTaskNameEmpty; }
            set
            {
                m_errorTaskNameEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorAnswerListEmpty
        {
            get { return m_errorAnswerListEmpty; }
            set
            {
                m_errorAnswerListEmpty = value;
                RaisePropertyChanged();
            }
        }

        public bool ErrorSomeAnswerEmpty
        {
            get { return m_errorSomeAnswerEmpty; }
            set
            {
                m_errorSomeAnswerEmpty = value;
                RaisePropertyChanged();
            }
        }

        public int AnswerColumn
        {
            get { return m_answerColumn; }
            set
            {
                m_answerColumn = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAnswerListEmpty
        {
            get { return AnswerList.Count == 0; }
        }

        private void DeleteAnswer(ItemViewModel viewModel)
        {
            AnswerList.Remove(viewModel);
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private void AddAnswer()
        {
            AnswerList.Add(new ItemViewModel());
            RaisePropertyChanged(() => IsAnswerListEmpty);
        }

        private void SaveTask()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ItemViewModel : ViewModelBase
    {
        private int m_shift;

        public ItemViewModel()
        {
            Shift = 0;

            ShiftLeftCommand = new RelayCommand(() =>
            {
                if (Shift > 0)
                    Shift--;
            });
            ShiftRightCommand = new RelayCommand(() =>
            {
                Shift++;
            });
        }

        public string Answer { get; set; }

        public string Label { get; set; }

        public RelayCommand ShiftLeftCommand { get; private set; }

        public RelayCommand ShiftRightCommand { get; private set; }

        public int Shift
        {
            get { return m_shift; }
            set
            {
                m_shift = value;
                RaisePropertyChanged();
            }
        }
    }
}
