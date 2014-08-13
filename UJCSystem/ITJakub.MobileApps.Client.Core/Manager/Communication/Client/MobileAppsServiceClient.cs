using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication.Client
{
    public class MobileAppsServiceClient : ClientBase<IMobileAppsService>, IMobileAppsService
    {
        public MobileAppsServiceClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {

        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken,
            UserDetailContract userDetail)
        {
            Channel.CreateUser(providerContract, providerToken, userDetail);
        }

        public LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            return Channel.LoginUser(providerContract, providerToken, email);
        }

        public UserGroupsContract GetGroupsByUser(long userId)
        {
            return Channel.GetGroupsByUser(userId);
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            return Channel.CreateGroup(userId, groupName);
        }

        #region enpoint settings
        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                var result = new BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                return new EndpointAddress("http://localhost/ITJakub.MobileApps.Service/MobileAppsService.svc");
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static Binding GetDefaultBinding()
        {
            return
                GetBindingForEndpoint(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }

        private static EndpointAddress GetDefaultEndpointAddress()
        {
            return GetEndpointAddress(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }
        #endregion

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            Channel.AddUserToGroup(groupAccessCode, userId);
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject)
        {
            Channel.CreateSynchronizedObject(applicationId, groupId, userId, synchronizedObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            return Channel.GetSynchronizedObjects(groupId, applicationId, objectType, since);
        }
    }
}