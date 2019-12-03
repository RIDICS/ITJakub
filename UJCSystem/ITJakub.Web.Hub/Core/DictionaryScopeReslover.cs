using ITJakub.Web.Hub.Options;
using Microsoft.Extensions.Options;

namespace ITJakub.Web.Hub.Core
{
    public class DictionaryScopeResolver
    {
        private const string CommunityTextsPrefix = "community";
        private readonly PortalOption m_portalOption;

        public DictionaryScopeResolver(IOptions<PortalOption> portalOption)
        {
            m_portalOption = portalOption.Value;
        }

        public string GetDictionaryScope(string dictionaryScope)
        {
            switch (m_portalOption.PortalType)
            {
                case PortalType.ResearchPortal:
                    return dictionaryScope;
                case PortalType.CommunityPortal:
                    return $"{CommunityTextsPrefix}-{dictionaryScope}";
                default:
                    return dictionaryScope;
            }
        }
    }
}