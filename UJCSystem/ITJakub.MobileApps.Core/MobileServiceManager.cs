using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        private readonly UserRepository m_userRepository;
        private SynchronizedObjectRepository m_synchronizedObjectRepository;
        private readonly InstitutionRepository m_institutionRepository;
        private readonly GroupRepository m_groupRepository;
        private readonly TaskRepository m_taskRepository;
        private readonly ApplicationRepository m_applicationRepository;

        public MobileServiceManager(IKernel container)
        {
            m_userRepository = container.Resolve<UserRepository>();
            m_synchronizedObjectRepository = container.Resolve<SynchronizedObjectRepository>();
            m_institutionRepository = container.Resolve<InstitutionRepository>();
            m_groupRepository = container.Resolve<GroupRepository>();
            m_taskRepository = container.Resolve<TaskRepository>();
            m_applicationRepository = container.Resolve<ApplicationRepository>();
        }

        public void CreateInstitution(Institution institution)
        {
            m_institutionRepository.Create(new DataEntities.Database.Entities.Institution()
            {
                Name = institution.Name,
                EnterCode = EnterCodeGenerator.GenerateCode(),
                CreateTime = DateTime.Now.ToUniversalTime()
            }); //TODO add check that generated code were unique (catch exception form DB)
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateUser(User user)
        {
            m_userRepository.Create(new DataEntities.Database.Entities.User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreateTime = DateTime.Now.ToUniversalTime()
            });
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