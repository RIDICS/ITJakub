using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using GalaSoft.MvvmLight.Threading;

namespace ITJakub.MobileApps.Client.MainApp.View.Control
{
    [ContentProperty(Name = "Content")]
    public sealed class NotificationControl : Windows.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (NotificationControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(NotificationControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof (bool), typeof (NotificationControl), new PropertyMetadata(false, OnVisibleChanged));
        
        public NotificationControl()
        {
            DefaultStyleKey = typeof(NotificationControl);
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public bool IsVisible
        {
            get { return (bool) GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var noficationControl = d as NotificationControl;
            if (noficationControl == null)
                return;

            var newState = noficationControl.IsVisible ? "Visible" : "Hidden";
            VisualStateManager.GoToState(noficationControl, newState, false);

            if (noficationControl.IsVisible)
            {
                Task.Delay(3000).ContinueWith(task => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (noficationControl.IsVisible)
                    {
                        noficationControl.IsVisible = false;
                    }
                }));
            }
        }
    }
}
