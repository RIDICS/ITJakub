﻿using System.Collections.Generic;
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
            UriTemplate = "institutions/create")]
        void CreateInstitution(Institution institution);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "institutions/{institutionId}")]
        InstitutionDetails GetInstitutionDetails(string institutionId);


        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/create?authprovider={authenticationProvider}&token={authenticationProviderToken}")]
        void CreateUser(string authenticationProvider, string authenticationProviderToken, User user);


        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "users/{userId}")]
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
            UriTemplate = "tasks/?app={applicationId}")]
        IEnumerable<TaskDetails> GetTasksForApplication(string applicationId);


        /// <summary>
        /// Saves predefined task by teacher to application
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "tasks/create?app={applicationId}&author={userId}")]
        void CreateTaskForApplication(string applicationId, string userId, Task task);


        /// <summary>
        /// Creates group of users in one application
        /// </summary>
        /// <param name="userId">Id of author</param>
        /// <param name="group">object describing users belongs to this group</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/create?author={userId}")]
        void CreateGroup(string userId, Group group);

        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "groups/{groupId}")]
        GroupDetails GetGroupDetails(string groupId);


        /// <summary>
        /// Returns collection of synchronized objects
        /// </summary>
        /// <param name="objectType">Determines kind of object (for possibility to retrive objects for app only of some kind like in Dictionary)</param>
        /// <param name="since">Time in UTC since objects we should return</param>
        /// <param name="groupId">Id of group where we are synchronizing</param>
        /// <param name="applicationId">Id of application which object belongs to</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "syncobjects/?group={groupId}&app={applicationId}&type={objectType}&since={since}")]
        IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string applicationId, string objectType, string since);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "syncobjects/create?group={groupId}&app={applicationId}&author={userId}")]
        void CreateSynchronizedObject(string groupId, string applicationId, string userId, SynchronizedObject synchronizedObject);
    }
}