using System;

namespace ITJakub.SearchService.Core.Exist
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExistAttribute : Attribute
    {
        public string XqueryName { get; set; }
    }
}