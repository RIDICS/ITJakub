using System;

namespace ITJakub.ITJakubService.Core
{
    public class CommunicationTokenGenerator
    {
        public string GetNewCommunicationToken()
        {
            return string.Format("CT:{0}",Guid.NewGuid()) ;
        }
    }
}