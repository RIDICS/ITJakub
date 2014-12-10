using System;
using System.Reflection;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist.AttributeResolver
{
    public interface IExistAttributeResolver
    {
        CommunicationInfo Resolve(ExistAttribute attribute, MethodInfo methodInfo);

        Type ResolvingAttributeType();
    }
}