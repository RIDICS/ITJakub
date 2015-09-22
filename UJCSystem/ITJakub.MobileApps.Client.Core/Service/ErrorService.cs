using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Threading;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class ErrorService : IErrorService
    {
        private MessageDialog m_messageDialog;
        private ErrorBar m_errorBar;

        public void ShowConnectionError(Action closeAction = null)
        {
            if (m_messageDialog != null)
                return;

            m_messageDialog = new MessageDialog("Nelze provést zvolenou akci. Prosím zkontrolujte připojení k internetu a akci opakujte.", "Nelze kontaktovat server");
            m_messageDialog.Commands.Add(new UICommand("Zavřít", command =>
            {
                Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(task => m_messageDialog = null);
                
                if (closeAction != null)
                    closeAction();
            }));

            DispatcherHelper.CheckBeginInvokeOnUI(() => m_messageDialog.ShowAsync());
        }

        public void ShowConnectionWarning()
        {
            if (m_errorBar != null || m_messageDialog != null)
                return;

            m_errorBar = new ErrorBar("Nelze kontaktovat server. Prosím zkontrolujte připojení k internetu.");
            m_errorBar.ClosedCommand = () =>
            {
                Task.Delay(new TimeSpan(0, 0, 30)).ContinueWith(task => m_errorBar = null);
            };

            DispatcherHelper.CheckBeginInvokeOnUI(m_errorBar.Show);
        }

        public void ShowError(string content, string title = null, Action closeAction = null)
        {
            if (m_messageDialog != null)
                return;

            m_messageDialog = title != null
                ? new MessageDialog(content, title)
                : new MessageDialog(content);
            
            m_messageDialog.Commands.Add(new UICommand("Zavřít", command =>
            {
                Task.Delay(new TimeSpan(0, 0, 1)).ContinueWith(task => m_messageDialog = null);
                
                if (closeAction != null)
                    closeAction();
            }));

            DispatcherHelper.CheckBeginInvokeOnUI(() => m_messageDialog.ShowAsync());
        }

        public void ShowDialog(MessageDialog messageDialog)
        {
            if (m_messageDialog != null)
                return;

            m_messageDialog = messageDialog;
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await m_messageDialog.ShowAsync();
                m_messageDialog = null;
            });
        }

        public void HideWarning()
        {
            if (m_errorBar != null)
                DispatcherHelper.CheckBeginInvokeOnUI(() => m_errorBar.Hide());
        }
    }
}
