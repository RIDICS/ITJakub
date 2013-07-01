using System.Collections.Generic;
using ITJakub.Core;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services.DTOs;

namespace ITJakub.MVCWebLayer.Services
{
    public class ItJakubSourcesProvider :ISourcesProvider
    {
        private readonly ItJakubServiceClient m_serviceClient;

        public ItJakubSourcesProvider()
        {
            m_serviceClient = Container.Current.Resolve<ItJakubServiceClient>();
        }

        public IEnumerable<Source> GetSearchResult()
        {
            return new List<Source>();
        }

        public Source GetDetail(string id)
        {
            return new Source();
        }

        public IEnumerable<Source> GetSources(string query, SourcesViewMode mode)
        {
            return new List<Source>();
        }
    }
}