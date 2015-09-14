using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Web;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataContracts.Tasks;
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

        public string GetBookLibraryEndpointAddress()
        {
            try
            {
                return m_serviceManager.GetBookLibraryEndpointAddress();
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
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

        public string GetSaltByUserEmail(string email)
        {
            try
            {
                return m_serviceManager.GetSaltByUserEmail(email);
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

        public SynchronizedObjectResponseContract GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since)
        {
            try
            {
                return m_serviceManager.GetLatestSynchronizedObject(groupId, applicationId, objectType, since);
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

        public IList<GroupDetailsUpdateContract> GetGroupsUpdate(IList<OldGroupDetailsContract> groups)
        {
            try
            {
                return m_serviceManager.GetGroupsUpdate(groups);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void AssignTaskToGroup(long groupId, long taskId)
        {
            try
            {
                m_serviceManager.AssignTaskToGroup(groupId, taskId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<TaskDetailContract> GetTasksByApplication(int applicationId)
        {
            try
            {
                return m_serviceManager.GetTasksByApplication(applicationId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public IList<TaskDetailContract> GetTasksByAuthor(long userId)
        {
            try
            {
                return m_serviceManager.GetTasksByAuthor(userId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void CreateTask(long userId, int applicationId, string name, string data, string description)
        {
            try
            {
                m_serviceManager.CreateTask(userId, applicationId, name, data, description);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public TaskDataContract GetTask(long taskId)
        {
            try
            {
                return m_serviceManager.GetTask(taskId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public TaskDataContract GetTaskForGroup(long groupId)
        {
            try
            {
                return m_serviceManager.GetTaskForGroup(groupId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public GroupStateContract GetGroupState(long groupId)
        {
            try
            {
                return m_serviceManager.GetGroupState(groupId);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void UpdateGroupState(long groupId, GroupStateContract state)
        {
            try
            {
                m_serviceManager.UpdateGroupState(groupId, state);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public void RemoveGroup(long groupId)
        {
            try
            {
                m_serviceManager.RemoveGroup(groupId);
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