using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthenticateResultInfo
    {
        public AuthResultType Result { get; set; }

        public string UserImageLocation { get; set; }
    }

    public enum AuthResultType
    {
        Success,
        Failed,
    }
}
