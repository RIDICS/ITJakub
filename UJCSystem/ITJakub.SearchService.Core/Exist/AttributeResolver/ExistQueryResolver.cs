using System;
using System.Reflection;
using System.Text;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist.AttributeResolver
{
    public class ExistQueryResolver : IExistAttributeResolver
    {
        private readonly IExistResourceManager m_existResourceManager;

        public ExistQueryResolver(IExistResourceManager existResourceManager)
        {
            m_existResourceManager = existResourceManager;
        }


        public CommunicationInfo Resolve(ExistAttribute attribute, MethodInfo methodInfo)
        {
            var att = (ExistQuery) attribute;
            string queryStringParams = GetQueryKeyValuePairTemplateFromMethodInfo(methodInfo);

            if (att.Method.Equals(HttpMethodType.Post))
            {
                return new CommunicationInfo
                {
                    UriTemplate = m_existResourceManager.GetQueryUri(att.XqueryName),
                    ContentTemplate = queryStringParams,
                    Method = att.Method,
                };
            }

            return new CommunicationInfo
            {
                UriTemplate = m_existResourceManager.GetQueryUriWithParams(att.XqueryName, queryStringParams),
                Method = att.Method
            };
        }

        public Type ResolvingAttributeType()
        {
            return typeof (ExistQuery);
        }

        private string GetQueryKeyValuePairTemplateFromMethodInfo(MethodInfo mInfo)
        {
            var stringBuilder = new StringBuilder();
            for (int index = 0; index < mInfo.GetParameters().Length; index++)
            {
                if (index != 0) stringBuilder.Append("&");
                ParameterInfo paramInfo = mInfo.GetParameters()[index];
                stringBuilder.Append(string.Format("{0}={{{1}}}", paramInfo.Name, index));
            }
            return stringBuilder.ToString();
        }
    }
}