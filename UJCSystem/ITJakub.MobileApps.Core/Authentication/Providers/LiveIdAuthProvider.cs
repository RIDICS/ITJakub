using System;
using System.Threading.Tasks;
using ITJakub.MobileApps.DataContracts;
using Microsoft.Live;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class LiveIdAuthProvider : IAuthProvider
    {
        public AuthProvidersContract ProviderContractType
        {
            get { return AuthProvidersContract.LiveId; }
        }

        public bool IsExternalProvider
        {
            get { return true; }
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {

            LiveAuthClient client = new LiveAuthClient("0000000048122D4E");//HACK temp id
            //LiveConnectClient cclient = new LiveConnectClient(client.Session);

            LiveConnectSession session = Task.Run(() => client.ExchangeAuthCodeAsync(accessToken)).Result;
            var cclient = new LiveConnectClient(session);

            LiveOperationResult meRs = Task.Run(() => cclient.GetAsync("me")).Result;

            var emails =  meRs.Result["wl.emails"];
            var image = meRs.Result["wl.image"];

            return new AuthenticateResultInfo {Result = AuthResultType.Success, UserImageLocation =(string) image};

        }

        
    }
}