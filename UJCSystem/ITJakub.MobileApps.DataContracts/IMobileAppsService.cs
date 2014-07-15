using System.ServiceModel;
using System.ServiceModel.Web;
using ITJakub.MobileApps.DataContracts.RequestObjects;
using ITJakub.MobileApps.DataContracts.ResponseObjects;

namespace ITJakub.MobileApps.DataContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        //TODO delete this test method
        [OperationContract]
        [WebInvoke(Method = "GET", 
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "json/{test}")]
        string TestMethod(string test);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/create")]
        CreateInstitutionResponse CreateInstitution(Institution institution);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/{institutionId}")]
        InstitutionDetailsResponse GetInstitutionDetails(string institutionId);


        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/{institutionId}/user/create")]
        CreateUserResponse CreateUser(string institutionId,User user);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institution/{institutionId}/user/{userId}")]
        UserDetailsResponse GetUserDetails(string institutionId,string userId);

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
        TasksForAppResponse GetTasksForApplication(string applicationId);



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
        CreateTaskForAppResponse CreateTaskForApplication(AppTask apptask);


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
            UriTemplate = "group/create")]
        CreateGroupResponse CreateGroup(Group group);


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
        SynchronizedObjectsResponse GetSynchronizedObjects(string groupId,string userId, string since);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "group/{groupId}/user/{userId}/syncobjects/create")]
        CreateSynchronizedObjectResponse CreateSynchronizedObject(string groupId, string userId,SynchronizedObject synchronizedObject);
        
    }
}