using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class SignInContract
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class SignInResultContract
    {
        public string CommunicationToken { get; set; }

        public List<string> Roles { get; set; }
    }
}