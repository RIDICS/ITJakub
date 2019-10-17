using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.ImportTestData.App
{
    public class ImportTestProjectManager
    {
        private readonly MainServiceProjectClient m_projectClient;

        public ImportTestProjectManager(MainServiceProjectClient projectClient)
        {
            m_projectClient = projectClient;
        }

        public void Import(int index)
        {
            // TODO create project, metadata, pages, texts
        }
    }
}