using System;

namespace ITJakub.SearchService.Core.Exist.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class ExistAttribute : System.Attribute
    {
        public string Method { get; set; }
    }
}