using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Hangman.ViewModel;

namespace ITJakub.MobileApps.Client.Hangman.View.Control
{
    public sealed partial class ResultTableControl
    {
        public ResultTableControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlayerRankingProperty = DependencyProperty.Register("PlayerRanking", typeof(ObservableCollection<ProgressInfoViewModel>), typeof(ResultTableControl), new PropertyMetadata(default(ObservableCollection<ProgressInfoViewModel>)));

        public ObservableCollection<ProgressInfoViewModel> PlayerRanking
        {
            get { return (ObservableCollection<ProgressInfoViewModel>)GetValue(PlayerRankingProperty); }
            set { SetValue(PlayerRankingProperty, value); }
        }
    }
}
