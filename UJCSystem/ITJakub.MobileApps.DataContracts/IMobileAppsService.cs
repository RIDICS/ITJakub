using System.ServiceModel;
using ITJakub.MobileApps.DataContracts.Applications;
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
        [AuthorizedMethod(UserRoleContract.Student)]
        UserGroupsContract GetGroupsByUser(long userId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        CreateGroupResponse CreateGroup(long userId , string groupName);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        void AddUserToGroup(string groupAccessCode, long userId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        void CreateSynchronizedObject(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject);
    }
}
