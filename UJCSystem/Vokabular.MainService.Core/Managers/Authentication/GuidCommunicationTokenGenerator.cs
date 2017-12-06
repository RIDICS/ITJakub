using System;
using Vokabular.DataEntities.Database.Entities;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class GuidCommunicationTokenGenerator : ICommunicationTokenGenerator
    {
        public string GetNewCommunicationToken(User dbUser)
        {
            return string.Format("CT:{0}", Guid.NewGuid());
        }
    }
}
