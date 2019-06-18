using System.Collections.Generic;

namespace Vokabular.Authentication.DataContracts
{
    public class BasicUserInfoContract : ContractBase
    {
        public int Id { get; set; }
        public Dictionary<string, string> UserData { get; set; }
    }
}