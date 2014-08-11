using System.ServiceModel;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.DataContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail);

        [OperationContract]
        LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email);

        [OperationContract]
        UserGroupsContract GetGroupsByUser(long userId);

        [OperationContract]
        CreateGroupResponse CreateGroup(long userId , string groupName);
    }
}
