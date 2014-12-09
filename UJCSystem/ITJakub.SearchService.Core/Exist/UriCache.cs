using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ITJakub.SearchService.Core.Exist
{
    public class UriCache
    {
        private readonly ExistConnectionSettingsSkeleton m_settings;
        private readonly Dictionary<string, string> m_uriTemplateDictionary;

        public UriCache(ExistConnectionSettingsSkeleton settings)
        {
            m_settings = settings;
            m_uriTemplateDictionary = new Dictionary<string, string>();
        }

        public string GetUriForMethod([CallerMemberName] string methodName = null)
        {
            string uriTemplate;
            m_uriTemplateDictionary.TryGetValue(methodName, out uriTemplate);
            if (string.IsNullOrEmpty(uriTemplate))
            {
                Type interfaceType = typeof (IExistClient);
                MethodInfo mInfo = interfaceType.GetMethod(methodName);
                var attribute = (ExistAttribute) Attribute.GetCustomAttribute(mInfo, typeof (ExistAttribute));
                string queryStringParams = GetQueryStringTemplateFromMethodInfo(mInfo);

                uriTemplate = string.Format("{0}{1}{2}{3}", m_settings.BaseUri, m_settings.XQueriesRelativeUri,
                    attribute.XqueryName, queryStringParams);
                AddUriForMethod(methodName, uriTemplate);
            }

            return uriTemplate;
        }

        private string GetQueryStringTemplateFromMethodInfo(MethodInfo mInfo)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("?");
            for (int index = 0; index < mInfo.GetParameters().Length; index++)
            {
                if (index != 0) stringBuilder.Append("&");
                ParameterInfo paramInfo = mInfo.GetParameters()[index];
                stringBuilder.Append(string.Format("{0}={{{1}}}", paramInfo.Name, index));
            }
            return stringBuilder.ToString();
        }

        private void AddUriForMethod(string methodName, string uriTemplate)
        {
            m_uriTemplateDictionary.Add(methodName, uriTemplate);
        }
    }
}