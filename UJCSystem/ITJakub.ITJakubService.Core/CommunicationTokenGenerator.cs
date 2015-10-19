using System;

namespace ITJakub.ITJakubService.Core
{
    public class CommunicationTokenGenerator
    {
        public string GetNewCommunicationToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}