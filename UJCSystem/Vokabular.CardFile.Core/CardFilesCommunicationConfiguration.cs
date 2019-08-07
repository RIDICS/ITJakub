using Vokabular.RestClient;

namespace Vokabular.CardFile.Core
{
    public class CardFilesCommunicationConfiguration : ServiceCommunicationConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
