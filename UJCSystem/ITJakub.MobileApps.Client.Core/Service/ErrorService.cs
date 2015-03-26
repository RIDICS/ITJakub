using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class ErrorService : IErrorService
    {
        private MessageDialog m_messageDialog;
        private ErrorBar m_errorBar;

        public void ShowConnectionError()
        {
            if (m_messageDialog != null)
                return;

            m_messageDialog = new MessageDialog("Nelze provést zvolenou akci. Prosím zkontrolujte připojení k internetu a akci opakujte.", "Nelze kontaktovat server");
            m_messageDialog.Commands.Add(new UICommand("Zavřít", command =>
            {
                m_messageDialog = null;
            }));
            m_messageDialog.ShowAsync();
        }

        public void ShowConnectionWarning()
        {
            if (m_errorBar != null || m_messageDialog != null)
                return;

            m_errorBar = new ErrorBar("Nelze kontaktovat server. Prosím zkontrolujte připojení k internetu.");
            m_errorBar.ClosedCommand = () =>
            {
                Task.Delay(new TimeSpan(0, 1, 0)).ContinueWith(task => m_errorBar = null);
            };
            m_errorBar.Show();
        }
    }
}
