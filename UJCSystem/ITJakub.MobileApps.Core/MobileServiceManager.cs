using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        private readonly StorageManager m_storageManager;

        public MobileServiceManager(IKernel container)
        {
            m_storageManager = container.Resolve<StorageManager>();
        }

        public void CreateInstitution(Institution institution)
        {
            m_storageManager.CreateInstitution(institution.Name, EnterCodeGenerator.GenerateCode(), DateTime.Now.ToUniversalTime()); //TODO add check that generated code were unique (catch exception form DB)
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public UserDetails GetUserDetails(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AppTaskDetails> GetTasksForApplication(string applicationId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateTaskForApplication(string applicationId, AppTask apptask)
        {
            throw new System.NotImplementedException();
        }

        public void CreateGroup(string institutionId, Group @group)
        {
            throw new System.NotImplementedException();
        }

        public GroupDetails GetGroupDetails(string groupId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string userId, string since)
        {
            throw new System.NotImplementedException();
        }

        public void CreateSynchronizedObject(string groupId, string userId, SynchronizedObject synchronizedObject)
        {
            throw new System.NotImplementedException();
        }
    }
}