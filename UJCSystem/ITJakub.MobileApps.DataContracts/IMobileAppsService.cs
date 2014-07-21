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
            UriTemplate = "institution/create")]
        void CreateInstitution(Institution institution);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/{institutionId}")]
        InstitutionDetails  GetInstitutionDetails(string institutionId);


        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "user/create")]
        void CreateUser(User user);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "user/{userId}")]
        UserDetails GetUserDetails(string userId);

        /// <summary>
        /// Returns predefined collection of task for specified application
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "application/{applicationId}/tasks")]
        IEnumerable<TaskDetails> GetTasksForApplication(string applicationId);



        /// <summary>
        /// Saves predefined task by teacher to application
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "application/{applicationId}/tasks/create")]
        void CreateTaskForApplication(string applicationId, Task apptask);


        /// <summary>
        /// Creates group of users in one application
        /// </summary>
        /// <param name="group">object describing users belongs to this group</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/{institutionId}/group/create")]
        void CreateGroup(string institutionId, Group group);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "group/{groupId}")]
        GroupDetails GetGroupDetails(string groupId);


        /// <summary>
        /// Returns collection of synchronized objects
        /// </summary>
        /// <param name="since">Time since objects we should return</param>
        /// <param name="groupId">GUID of group where we are synchronizing</param>
        /// <param name="userId">GUID of group where we are synchronizing</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "group/{groupId}/user/{userId}/syncobjects/{since}")]
        IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string userId, string since);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "group/{groupId}/user/{userId}/syncobjects/create")]
        void CreateSynchronizedObject(string groupId, string userId,SynchronizedObject synchronizedObject);
        
    }
}