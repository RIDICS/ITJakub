using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class ITJakubManager : IAuthProvider
    {
        public override Task<UserInfo> LoginAsync()
        {
            throw new NotImplementedException();
        }
    }
}
