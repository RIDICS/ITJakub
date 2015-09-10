using System;
using Jewelery;

namespace ITJakub.SearchService.Core.Exist.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class ExistAttribute : Attribute
    {
        public HttpMethodType Method { get; set; }
    }

    public enum HttpMethodType
    {
        [StringValue("GET")]
        Get = 0,
        [StringValue("PUT")]
        Put = 1,
        [StringValue("POST")]
        Post = 2,
        [StringValue("DELETE")]
        Delete = 3
    }
}