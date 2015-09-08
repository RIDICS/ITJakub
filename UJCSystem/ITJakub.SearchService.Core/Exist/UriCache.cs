using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    public class UriCache
    {
        private readonly object m_lock = new object();
        private readonly MethodInfoResolver m_methodInfoResolver;
        private readonly ExistConnectionSettingsSkeleton m_settings;
        private readonly Dictionary<string, CommunicationInfo> m_uriTemplateDictionary;

        public UriCache(ExistConnectionSettingsSkeleton settings, MethodInfoResolver methodInfoResolver)
        {
            m_settings = settings;
            m_methodInfoResolver = methodInfoResolver;
            m_uriTemplateDictionary = new Dictionary<string, CommunicationInfo>();
        }

        public CommunicationInfo GetCommunicationInfoForMethod([CallerMemberName] string methodName = null)
        {
            CommunicationInfo commInfo;
            if (methodName == null) return null;
            lock (m_lock)
            {
                m_uriTemplateDictionary.TryGetValue(methodName, out commInfo);
                if (commInfo == null)
                {
                    var interfaceType = typeof (IExistClient);
                    var mInfo = interfaceType.GetMethod(methodName);
                    commInfo = m_methodInfoResolver.Resolve(mInfo);
                    AddCommunicationInfoForMethod(methodName, commInfo);
                }
            }

            return commInfo;
        }

        private void AddCommunicationInfoForMethod(string methodName, CommunicationInfo communicationInfo)
        {
            m_uriTemplateDictionary.Add(methodName, communicationInfo);
        }
    }

    public class CommunicationInfo
    {
        public HttpMethodType Method { get; set; }
        public string UriTemplate { get; set; }
        public string ContentTemplate { get; set; }
    }
}