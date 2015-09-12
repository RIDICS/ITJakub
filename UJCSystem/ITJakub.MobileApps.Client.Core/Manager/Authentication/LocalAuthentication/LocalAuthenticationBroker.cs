using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.LocalAuthentication
{
    public class LocalAuthenticationBroker
    {
        private TaskCompletionSource<bool> m_taskCompletion;
        private UserLoginSkeletonWithPassword m_userLoginSkeleton;
        private Popup m_popup;
        private LocalAuthView m_view;
        private LocalAuthViewModel m_viewModel;

        public bool IsCreatingUser { get; private set; }

        public async Task<UserLoginSkeletonWithPassword> LoginAsync()
        {
            IsCreatingUser = false;
            m_taskCompletion = new TaskCompletionSource<bool>();
            return await StartAndGetUserInfoAsync(false);
        }

        public async Task<UserLoginSkeletonWithPassword> CreateUserAsync()
        {
            IsCreatingUser = true;
            m_taskCompletion = new TaskCompletionSource<bool>();
            return await StartAndGetUserInfoAsync(true);
        }

        public async Task<UserLoginSkeletonWithPassword> ReopenWithErrorAsync()
        {
            m_viewModel.ShowAuthenticationError();
            OpenPopup();

            m_taskCompletion = new TaskCompletionSource<bool>();
            await m_taskCompletion.Task;
            return m_userLoginSkeleton;
        }

        private async Task<UserLoginSkeletonWithPassword> StartAndGetUserInfoAsync(bool createNewUser)
        {
            m_viewModel = new LocalAuthViewModel
            {
                ShowCreateControls = createNewUser,
                ShowLoginControls = !createNewUser
            };
            m_view = new LocalAuthView
            {
                DataContext = m_viewModel,
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };

            OpenPopup();

            await m_taskCompletion.Task;
            return m_userLoginSkeleton;
        }

        private void OpenPopup()
        {
            m_popup = new Popup
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = m_view
            };

            InputPane.GetForCurrentView().Showing += KeyboardShowing;
            InputPane.GetForCurrentView().Hiding += KeyboardHiding;
            Window.Current.SizeChanged += UpdatePageSize;

            Messenger.Default.Register<LocalAuthCompletedMessage>(this, OnLocalAuthCompleted);
            m_popup.IsOpen = true;
        }
        
        private void OnLocalAuthCompleted(LocalAuthCompletedMessage message)
        {
            m_popup.IsOpen = false;
            m_popup.Child = null;
            m_popup = null;
            m_userLoginSkeleton = message.UserLoginSkeleton;
            m_taskCompletion.SetResult(true);
            Messenger.Default.Unregister(this);

            InputPane.GetForCurrentView().Hiding -= KeyboardHiding;
            InputPane.GetForCurrentView().Showing -= KeyboardShowing;
            Window.Current.SizeChanged -= UpdatePageSize;
        }

        private void KeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_view.Height = Window.Current.Bounds.Height - args.OccludedRect.Height;
        }

        private void KeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            m_view.Height = Window.Current.Bounds.Height;
        }

        private void UpdatePageSize(object sender, WindowSizeChangedEventArgs e)
        {
            var page = m_popup.Child as Page;
            if (page == null)
                return;

            page.Width = Window.Current.Bounds.Width;
            page.Height = Window.Current.Bounds.Height;
        }
    }
}
