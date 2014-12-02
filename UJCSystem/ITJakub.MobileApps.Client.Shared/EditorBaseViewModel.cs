using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class EditorBaseViewModel : ViewModelBase
    {
        public void GoBack()
        {
            ((Frame) Window.Current.Content).GoBack();
        }
    }
}
