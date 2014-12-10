using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ITJakub.SearchService.Core.Exist
{
    public class UriCache
    {
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
            if (methodName == null) return null;
            CommunicationInfo commInfo;
            m_uriTemplateDictionary.TryGetValue(methodName, out commInfo);
            if (commInfo == null)
            {
                Type interfaceType = typeof (IExistClient);
                MethodInfo mInfo = interfaceType.GetMethod(methodName);
                commInfo = m_methodInfoResolver.Resolve(mInfo);
                AddCommunicationInfoForMethod(methodName, commInfo);
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
        public string Method { get; set; }
        public string UriTemplate { get; set; }
    }
}