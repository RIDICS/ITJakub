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
        private readonly SynchronizedObjectRepository m_synchronizedObjectRepository;
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

        public void CreateUser(string authenticationProvider, string authenticationProviderToken, User user)
        {
            var deUser = AutoMapper.Mapper.Map<DE.User>(user);
            deUser.CreateTime = DateTime.UtcNow;
            deUser.AuthenticationProvider = (byte)Enum.Parse(typeof(AuthenticationProvider), authenticationProvider);
            deUser.AuthenticationProviderToken = authenticationProviderToken;
            m_userRepository.Create(deUser); //TODO add check that pair (email,authenticationProvider) were unique (catch exception form DB)
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

        public void CreateTaskForApplication(string applicationId, string userId, Task apptask)
        {
            var application = m_applicationRepository.FindById(long.Parse(applicationId));
            var user = m_userRepository.FindById(long.Parse(userId));
            var deTask = AutoMapper.Mapper.Map<DE.Task>(apptask);
            deTask.Application = application;
            deTask.Author = user;
            deTask.CreateTime = DateTime.UtcNow;
            deTask.Guid = Guid.NewGuid().ToString();
            m_taskRepository.Create(deTask);
            //TODO save appTask.Data to Azure tables here under deTask.Application.Id as partition key and deTask.GUID as row key
        }

        public void CreateGroup(string userId, Group group)
        {
            var user = m_userRepository.FindById(long.Parse(userId));
            var deGroup = AutoMapper.Mapper.Map<DE.Group>(group);
            deGroup.Author = user;
            deGroup.CreateTime = DateTime.UtcNow;
            deGroup.EnterCode = EnterCodeGenerator.GenerateCode();
            m_groupRepository.Create(deGroup);
        }

        public GroupDetails GetGroupDetails(string groupId)
        {
            var group = m_groupRepository.LoadGroupWithDetails(long.Parse(groupId));
            return AutoMapper.Mapper.Map<GroupDetails>(group);
        }

        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string applicationId, string objectType, string since)
        {
            throw new NotImplementedException();
        }

        public void CreateSynchronizedObject(string groupId, string applicationId, string userId, SynchronizedObject synchronizedObject)
        {
            var application = m_applicationRepository.FindById(long.Parse(applicationId));
            var user = m_userRepository.FindById(long.Parse(userId));
            var group = m_groupRepository.FindById(long.Parse(userId));
            var deSyncObject = AutoMapper.Mapper.Map<DE.SynchronizedObject>(synchronizedObject);
            deSyncObject.Application = application;
            deSyncObject.Author = user;
            deSyncObject.Group = group;
            deSyncObject.CreateTime = DateTime.UtcNow;
            deSyncObject.Guid = Guid.NewGuid().ToString();
            m_synchronizedObjectRepository.Create(deSyncObject);
            //TODO save SyncObject.Data to Azure tables here under deSyncObject.Group.Id as partition key and deSyncObject.GUID as row key
        }
    }
}