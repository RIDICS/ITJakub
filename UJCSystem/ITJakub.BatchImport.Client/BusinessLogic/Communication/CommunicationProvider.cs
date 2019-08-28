using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.BatchImport.Client.BusinessLogic.Communication
{
    public class CommunicationProvider
    {
        private readonly MainServiceSessionClient m_mainServiceSessionClient;

        public CommunicationProvider(MainServiceSessionClient mainServiceSessionClient)
        {
            m_mainServiceSessionClient = mainServiceSessionClient;
        }

        public MainServiceSessionClient GetMainServiceSessionClient()
        {
            return m_mainServiceSessionClient;
        }
    }
}
