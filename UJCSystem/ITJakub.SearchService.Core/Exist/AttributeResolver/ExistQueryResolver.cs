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
            string queryStringParams = GetQueryStringTemplateFromMethodInfo(methodInfo);
            var commInfo = new CommunicationInfo
            {
                UriTemplate = m_existResourceManager.GetQueryUriTemplate(att.XqueryName, queryStringParams),
                Method = att.Method
            };
            return commInfo;
        }

        public Type ResolvingAttributeType()
        {
            return typeof (ExistQuery);
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
    }
}