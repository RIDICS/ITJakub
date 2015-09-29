using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using WinRTXamlToolkit.Controls.Extensions;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    /// <summary>
    /// Using MVVM to close a flyout
    /// </summary>
    public class CloseFlyoutAction : DependencyObject, IAction
    {
        /// <inheritdoc/>
        public object Execute(object sender, object parameter)
        {
            var element = sender as DependencyObject;
            var flyout = element.GetFirstAncestorOfType<FlyoutPresenter>();
            var popup = flyout.Parent as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
            return null;
        }
    }
}