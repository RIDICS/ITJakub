using System;
using ITJakub.SearchService.DataContracts.Types;

namespace ITJakub.SearchService.Core.Exist.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExistResource : ExistAttribute
    {
        public ResourceLevelEnumContract Type { get; set; }
    }
}