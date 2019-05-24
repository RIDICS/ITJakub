using System;

namespace Vokabular.Authentication.Client.Model
{
    public class TokenData
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}