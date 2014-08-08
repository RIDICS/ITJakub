using System.ServiceModel;

namespace ITJakub.MobileApps.DataContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail);

        [OperationContract]
        LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email);

    }
}
