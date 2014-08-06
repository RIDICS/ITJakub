using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ITJakub.MobileApps.DataContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/create?token={authenticationProviderToken}")]
        void CreateUser(string authenticationProviderToken, AuthenticationProviders authenticationProvider, User user);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/login")]
        LoginUserResponse LoginUser(UserLogin userLogin);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institutions/create")]
        [AuthorizedMethod(UserRole.Student)]
        void CreateInstitution(Institution institution);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institutions/{institutionId}")]
        [AuthorizedMethod(UserRole.Student)]
        InstitutionDetails GetInstitutionDetails(string institutionId);

        /// <summary>
        ///     Add user to institution
        /// </summary>
        /// <param name="userId">userId which want to enter institution</param>
        /// <param name="enterCode">Code generated at time of creating instituion (password)</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institutions/enter?code={enterCode}&user={userId}")]
        [AuthorizedMethod(UserRole.Student,"userId")]
        void AddUserToInstitution(string enterCode, string userId);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/{userId}")]
        [AuthorizedMethod(UserRole.Student)]
        UserDetails GetUserDetails(string userId);

        /// <summary>
        ///     Returns collection of task made by user
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/{userId}/tasks")]
        [AuthorizedMethod(UserRole.Teacher)]
        IEnumerable<TaskDetails> GetTasksByUser(string userId);


        /// <summary>
        ///     Returns collection of groups made by user
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/{userId}/groups")]
        [AuthorizedMethod(UserRole.Student)]
        IEnumerable<GroupDetails> GetGroupsByUser(string userId);

        /// <summary>
        ///     Returns collection of groups where user belongs to
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/{userId}/memberships")]
        [AuthorizedMethod(UserRole.Student)]
        IEnumerable<GroupDetails> GetMembershipsForUser(string userId);


        /// <summary>
        ///     Returns predefined collection of task for specified application
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "tasks/search?app={applicationId}")]
        [AuthorizedMethod(UserRole.Teacher)]
        IEnumerable<TaskDetails> GetTasksForApplication(string applicationId);


        /// <summary>
        ///     Saves predefined task by teacher to application
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "tasks/create?app={applicationId}&author={userId}")]
        [AuthorizedMethod(UserRole.Teacher, "userId")]
        void CreateTaskForApplication(string applicationId, string userId, Task task);


        /// <summary>
        ///     Creates group of users in one application
        /// </summary>
        /// <param name="userId">Id of author</param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/create?author={userId}&name={groupName}")]
        [AuthorizedMethod(UserRole.Teacher, "userId")]
        CreateGroupResponse CreateGroup(string userId, string groupName);

        /// <summary>
        ///     Assign task to group
        /// </summary>
        /// <param name="groupId">Id of group</param>
        /// <param name="userId">userId which should be same as authorId</param>
        /// <param name="taskId">Id of task to be assigned</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/{groupId}/assign?task={taskId}&user={userId}")]
        [AuthorizedMethod(UserRole.Teacher, "userId")]
        void AssignTaskToGroup(string groupId, string taskId, string userId);


        /// <summary>
        ///     Add user to group
        /// </summary>
        /// <param name="userId">userId which want to enter group</param>
        /// <param name="enterCode">Code generated at time of creating group (password)</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/enter?code={enterCode}&user={userId}")]
        [AuthorizedMethod(UserRole.Student, "userId")]
        void AddUserToGroup(string enterCode, string userId);

        /// <summary>
        ///     Return detail information about group
        /// </summary>
        /// <param name="groupId">ID of group</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/{groupId}")]
        [AuthorizedMethod(UserRole.Student)]
        GroupDetails GetGroupDetails(string groupId);


        /// <summary>
        ///     Returns collection of synchronized objects
        /// </summary>
        /// <param name="objectType">
        ///     Determines kind of object (for possibility to retrive objects for app only of some kind like
        ///     in Dictionary)
        /// </param>
        /// <param name="since">Time in UTC since objects we should return</param>
        /// <param name="groupId">Id of group where we are synchronizing</param>
        /// <param name="applicationId">Id of application which object belongs to</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "syncobjects/search?group={groupId}&app={applicationId}&type={objectType}&since={since}")]
        [AuthorizedMethod(UserRole.Student)]
        IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string applicationId, string objectType, string since);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "syncobjects/create?group={groupId}&app={applicationId}&author={userId}")]
        [AuthorizedMethod(UserRole.Student, "userId")]
        void CreateSynchronizedObject(string groupId, string applicationId, string userId, SynchronizedObject synchronizedObject);
    }
}