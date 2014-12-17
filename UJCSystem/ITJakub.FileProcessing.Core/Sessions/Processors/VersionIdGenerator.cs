using System;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class VersionIdGenerator
    {
        public string Generate(DateTime createTime)
        {
            return Convert.ToString(Guid.NewGuid());
        }
    }
}