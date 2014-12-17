using Windows.UI.Xaml.Controls.Primitives;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public interface INavigationService
    {
        Popup ParentPopup { get; }
        bool CanGoBack();
        void GoBack();
        void Navigate<T>();
        void ResetBackStack();
    }
}