using System.Collections.Generic;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_manager;

        public MobileAppsService()
        {
            m_manager = Container.Current.Resolve<IMobileAppsService>();
        }
 

        public void CreateInstitution(Institution institution)
        {
             m_manager.CreateInstitution(institution);
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            return m_manager.GetInstitutionDetails(institutionId);
        }


        public void CreateUser(User user)
        {
            m_manager.CreateUser(user);
        }

        public UserDetails GetUserDetails(string userId)
        {
            return m_manager.GetUserDetails(userId);
        }

        public IEnumerable<TaskDetails> GetTasksForApplication(string applicationId)
        {
            return m_manager.GetTasksForApplication(applicationId);
        }

        public void CreateTaskForApplication(string applicationId, Task apptask)
        {
            m_manager.CreateTaskForApplication(applicationId,apptask);
        }

        public void CreateGroup(string institutionId, Group group)
        {
            m_manager.CreateGroup(institutionId,group);
        }

        public GroupDetails GetGroupDetails(string groupId)
        {
            return m_manager.GetGroupDetails(groupId);
        }

        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string userId, string since)
        {
            return m_manager.GetSynchronizedObjects(groupId, userId, since);
        }

        public void CreateSynchronizedObject(string groupId, string userId, SynchronizedObject synchronizedObject)
        {
            m_manager.CreateSynchronizedObject(groupId, userId, synchronizedObject);
        }
    }
}
