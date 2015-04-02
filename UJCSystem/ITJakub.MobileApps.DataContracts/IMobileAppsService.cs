using System;
using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataContracts.Tasks;

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
        [FaultContract(typeof(ApplicationNotRunningFault))]
        void CreateSynchronizedObject(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        [FaultContract(typeof(ApplicationNotRunningFault))]
        IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        [FaultContract(typeof(ApplicationNotRunningFault))]
        SynchronizedObjectResponseContract GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since);

        [OperationContract]
        IList<ApplicationContract> GetAllApplication();

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        GroupDetailContract GetGroupDetails(long groupId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        IList<GroupDetailsUpdateContract> GetGroupsUpdate(IList<OldGroupDetailsContract> groups);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        void AssignTaskToGroup(long groupId, long taskId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        IList<TaskDetailContract> GetTasksByApplication(int applicationId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        IList<TaskDetailContract> GetTasksByAuthor(long userId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        void CreateTask(long userId, int applicationId, string name, string data);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        TaskContract GetTaskForGroup(long groupId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Student)]
        GroupStateContract GetGroupState(long groupId);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        void UpdateGroupState(long groupId, GroupStateContract state);

        [OperationContract]
        [AuthorizedMethod(UserRoleContract.Teacher)]
        void RemoveGroup(long groupId);
    }
}
