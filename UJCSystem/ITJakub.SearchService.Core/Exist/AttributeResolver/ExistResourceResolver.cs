using System;
using System.Reflection;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist.AttributeResolver
{
    public class ExistResourceResolver : IExistAttributeResolver
    {
        private readonly IExistResourceManager m_existResourceManager;

        public ExistResourceResolver(IExistResourceManager existResourceManager)
        {
            m_existResourceManager = existResourceManager;
        }

        public CommunicationInfo Resolve(ExistAttribute attribute, MethodInfo methodInfo)
        {
            var att = (ExistResource) attribute;
            var commInfo = new CommunicationInfo {Method = att.Method, UriTemplate = m_existResourceManager.GetResourceUriTemplate(att.Type)};
            return commInfo;
        }

        public Type ResolvingAttributeType()
        {
            return typeof (ExistResource);
        }
    }
}