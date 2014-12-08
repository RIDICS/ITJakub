using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistConnectionSettingsSkeleton m_existSettings;
        private readonly ExistDaoClient m_client;

        public ExistManager(ExistConnectionSettingsSkeleton existSettings)
        {
            m_existSettings = existSettings;
            m_client = new ExistDaoClient();
        }

        public string GetPageList(string documentId)
        {
            return m_client.GetPageList(m_existSettings.ViewsCollection, documentId);
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition)
        {
            throw new System.NotImplementedException();
        }

        public string GetPageByName(string documentId, string pageName)
        {
            throw new System.NotImplementedException();
        }

        public string GetPagesByName(string documentId, string start, string end)
        {
            throw new System.NotImplementedException();
        }
    }
}