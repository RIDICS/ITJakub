using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using ITJakub.MobileApps.Client.Core;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp.View;
using ITJakub.MobileApps.Client.MainApp.ViewModel.Message;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.MainApp
{
    public class ApplicationSelector
    {
        private readonly TaskCompletionSource<AppInfoViewModel> m_taskCompletition;
        private readonly INavigationService m_navigationService;

        private ApplicationSelector()
        {
            Messenger.Default.Register<SelectedApplicationMessage>(this, ApplicationSelected);

            m_taskCompletition = new TaskCompletionSource<AppInfoViewModel>();
            m_navigationService = Container.Current.Resolve<INavigationService>();
        }

        private async Task<AppInfoViewModel> StartSelectingApplicationAsync()
        {
            m_navigationService.Navigate(typeof(ApplicationSelectionView));
            return await m_taskCompletition.Task;
        } 

        private void ApplicationSelected(SelectedApplicationMessage message)
        {
            Messenger.Default.Unregister(this);
            m_taskCompletition.SetResult(message.AppInfo);
        }

        public static async Task<AppInfoViewModel> SelectApplicationAsync()
        {
            var book = new ApplicationSelector();
            return await book.StartSelectingApplicationAsync();
        } 
    }
}
