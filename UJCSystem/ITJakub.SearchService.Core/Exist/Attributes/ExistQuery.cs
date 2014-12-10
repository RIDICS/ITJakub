using System;

namespace ITJakub.SearchService.Core.Exist.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExistQuery : ExistAttribute
    {
        public string XqueryName { get; set; }

        public new string Method
        {
            get { return "GET"; }
        }
    }
}