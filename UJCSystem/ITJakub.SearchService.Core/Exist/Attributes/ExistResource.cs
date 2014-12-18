using System;

namespace ITJakub.SearchService.Core.Exist.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExistResource : ExistAttribute
    {
        public ResourceLevelType Type { get; set; }
    }

    //Determines directory level in exist db
    public enum ResourceLevelType
    {
        Version = 0,
        Book = 1,
        Shared = 2
    }
}