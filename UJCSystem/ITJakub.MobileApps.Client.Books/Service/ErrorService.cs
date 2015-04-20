using ITJakub.MobileApps.Client.Books.View.Control;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class ErrorService : IErrorService
    {
        private ErrorBar m_errorBar;

        public void ShowCommunicationWarning()
        {
            if (m_errorBar != null)
                return;

            m_errorBar = new ErrorBar("Nelze kontaktovat server. Prosím zkontrolujte připojení k internetu a akci zopakujte.");
            m_errorBar.ClosedCommand = () =>
            {
                m_errorBar = null;
            };

            m_errorBar.Show();
        }
    }
}
