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
            return m_portalOption.PortalType == PortalType.CommunityPortal && !dictionaryScope.Contains(CommunityTextsPrefix) 
                ? $"{CommunityTextsPrefix}-{dictionaryScope}" 
                : dictionaryScope;
        }
    }
}