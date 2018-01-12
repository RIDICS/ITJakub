using Vokabular.DataEntities.Database.Entities;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public interface ICommunicationTokenGenerator
    {
        string GetNewCommunicationToken(User dbUser);
        bool ValidateTokenFormat(string token);
    }
}