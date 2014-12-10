using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel;
using ITJakub.SearchService.Core.Exist.AttributeResolver;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    public class MethodInfoResolver
    {
        private readonly Dictionary<Type, IExistAttributeResolver> m_attributeResolverDictionary;

        public MethodInfoResolver(IKernel container)
        {
            m_attributeResolverDictionary = new Dictionary<Type, IExistAttributeResolver>();
            foreach (var resolver in container.ResolveAll<IExistAttributeResolver>())
            {
                m_attributeResolverDictionary.Add(resolver.ResolvingAttributeType(), resolver);
            }
        }

        public CommunicationInfo Resolve(MethodInfo methodInfo)
        {
            var attribute = (ExistAttribute) Attribute.GetCustomAttribute(methodInfo, typeof (ExistAttribute));
            IExistAttributeResolver resolver;
            m_attributeResolverDictionary.TryGetValue(attribute.GetType(), out resolver);
            return resolver == null ? null : resolver.Resolve(attribute, methodInfo);
        }
    }
}