using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class EditorBaseViewModel : ViewModelBase
    {
        public RelayCommand GoBackCommand
        {
            get { return new RelayCommand(((Frame)Window.Current.Content).GoBack); }
        }
    }
}
