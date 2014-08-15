using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Web;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;
using log4net;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_serviceManager;
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MobileAppsService()
        {
            m_serviceManager = Container.Current.Resolve<IMobileAppsService>();
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            try
            {
                m_serviceManager.CreateUser(providerContract, providerToken, userDetail);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            try
            {
                return m_serviceManager.LoginUser(providerContract, providerToken, email);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public UserGroupsContract GetGroupsByUser(long userId)
        {
            try
            {
                return m_serviceManager.GetGroupsByUser(userId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            try
            {
                return m_serviceManager.CreateGroup(userId, groupName);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            try
            {
                m_serviceManager.AddUserToGroup(groupAccessCode, userId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            try
            {
                m_serviceManager.CreateSynchronizedObject(applicationId, groupId, userId, synchronizedObject);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            try
            {
                return m_serviceManager.GetSynchronizedObjects(groupId, applicationId, objectType, since);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            try
            {
                return m_serviceManager.GetAllApplication();
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public GroupDetailContract GetGroupDetails(long groupId)
        {
            try
            {
                return m_serviceManager.GetGroupDetails(groupId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<GroupMemberContract> GetGroupMembers(long groupId)
        {
            try
            {
                return m_serviceManager.GetGroupMembers(groupId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<long> GetGroupMemberIds(long groupId)
        {
            try
            {
                return m_serviceManager.GetGroupMemberIds(groupId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }
    }
}