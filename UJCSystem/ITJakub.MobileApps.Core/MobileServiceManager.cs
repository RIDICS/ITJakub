using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

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
            var deInstitution = AutoMapper.Mapper.Map<DE.Institution>(institution);
            deInstitution.EnterCode = EnterCodeGenerator.GenerateCode();
            deInstitution.CreateTime = DateTime.UtcNow;
            m_institutionRepository.Create(deInstitution); //TODO add check that generated enterCode were unique (catch exception form DB)
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            var institution = m_institutionRepository.LoadInstitutionWithDetails(long.Parse(institutionId));
            return AutoMapper.Mapper.Map<InstitutionDetails>(institution); 
        }

        public void CreateUser(User user)
        {
            var deUser = AutoMapper.Mapper.Map<DE.User>(user);
            deUser.CreateTime = DateTime.UtcNow;
            m_userRepository.Create(deUser); //TODO add check that email were unique (catch exception form DB)
        }

        public UserDetails GetUserDetails(string userId)
        {
            var user = m_userRepository.LoadUserWithDetails(long.Parse(userId));
            return AutoMapper.Mapper.Map<UserDetails>(user);
        }

        public IEnumerable<TaskDetails> GetTasksForApplication(string applicationId)
        {
            var application = m_applicationRepository.FindById(long.Parse(applicationId));
            var tasks = m_taskRepository.LoadTasksWithDetailsByApplication(application);
            return AutoMapper.Mapper.Map<IList<TaskDetails>>(tasks);
        }

        public void CreateTaskForApplication(string applicationId, Task apptask)
        {
            var application = m_applicationRepository.FindById(long.Parse(applicationId));
            var deTask = AutoMapper.Mapper.Map<DE.Task>(apptask);
            deTask.Application = application;
            m_taskRepository.Create(deTask);
        }

        public void CreateGroup(string institutionId, Group group)
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