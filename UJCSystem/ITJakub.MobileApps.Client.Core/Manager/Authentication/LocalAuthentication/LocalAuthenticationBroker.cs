using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.LocalAuthentication
{
    public class LocalAuthenticationBroker
    {
        private readonly TaskCompletionSource<bool> m_taskCompletion;
        private UserLoginSkeletonWithPassword m_userLoginSkeleton;
        private Popup m_popup;
        private LocalAuthView m_view;

        public LocalAuthenticationBroker()
        {
            m_taskCompletion = new TaskCompletionSource<bool>();
        }

        public static async Task<UserLoginSkeletonWithPassword> LoginAsync()
        {
            var localAuthentication = new LocalAuthenticationBroker();
            return await localAuthentication.StartAndGetUserInfoAsync(false);
        }

        public static async Task<UserLoginSkeletonWithPassword> CreateUserAsync()
        {
            var localAuthentication = new LocalAuthenticationBroker();
            return await localAuthentication.StartAndGetUserInfoAsync(true);
        }

        private async Task<UserLoginSkeletonWithPassword> StartAndGetUserInfoAsync(bool createNewUser)
        {
            var viewModel = new LocalAuthViewModel
            {
                ShowCreateControls = createNewUser,
                ShowLoginButton = !createNewUser
            };
            m_view = new LocalAuthView
            {
                DataContext = viewModel,
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };
            m_popup = new Popup
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = m_view
            };

            InputPane.GetForCurrentView().Showing += KeyboardShowing;
            InputPane.GetForCurrentView().Hiding += KeyboardHiding;

            Messenger.Default.Register<LocalAuthCompletedMessage>(this, OnLocalAuthCompleted);
            m_popup.IsOpen = true;

            await m_taskCompletion.Task;
            return m_userLoginSkeleton;
        }
        
        private void OnLocalAuthCompleted(LocalAuthCompletedMessage message)
        {
            m_popup.IsOpen = false;
            m_popup = null;
            m_userLoginSkeleton = message.UserLoginSkeleton;
            m_taskCompletion.SetResult(true);
            Messenger.Default.Unregister(this);

            InputPane.GetForCurrentView().Hiding -= KeyboardHiding;
            InputPane.GetForCurrentView().Showing -= KeyboardShowing;
        }

        private void KeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_view.Height = Window.Current.Bounds.Height - args.OccludedRect.Height;
        }

        private void KeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_view.Height = Window.Current.Bounds.Height;
        }
    }
}
