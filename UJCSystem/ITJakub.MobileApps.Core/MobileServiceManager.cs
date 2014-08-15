//using System;
//using System.Collections.Generic;
//using AutoMapper;
//using Castle.MicroKernel;
//using ITJakub.MobileApps.DataContracts;
//using ITJakub.MobileApps.DataEntities;
//using ITJakub.MobileApps.DataEntities.AzureTables.Daos;
//using ITJakub.MobileApps.DataEntities.AzureTables.Entities;
//using ITJakub.MobileApps.DataEntities.Database.Repositories;
//using DE = ITJakub.MobileApps.DataEntities.Database.Entities;

using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.Core.Applications;
using ITJakub.MobileApps.Core.Groups;
using ITJakub.MobileApps.Core.Users;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        private readonly UserManager m_userManager;
        private readonly GroupManager m_groupManager;
        private readonly ApplicationManager m_applicationManager;

        public MobileServiceManager(IKernel container)
        {

            m_userManager = container.Resolve<UserManager>();
            m_groupManager = container.Resolve<GroupManager>();
            m_applicationManager = container.Resolve<ApplicationManager>();
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            m_userManager.CreateUser(providerContract, providerToken, userDetail);
        }

        public LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            return m_userManager.LoginUser(providerContract, providerToken, email);
        }


        public UserGroupsContract GetGroupsByUser(long userId)
        {
            return m_groupManager.GetGroupByUser(userId);
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            return m_groupManager.CreateGroup(userId, groupName);
        }

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            m_groupManager.AddUserToGroup(groupAccessCode, userId);
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            m_applicationManager.CreateSynchronizedObject(applicationId, groupId, userId, synchronizedObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            return m_applicationManager.GetSynchronizedObjects(groupId, applicationId, objectType, since);
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            return m_applicationManager.GetAllApplication();
        }

        public GroupDetailContract GetGroupDetails(long groupId)
        {
            return m_groupManager.GetGroupDetails(groupId);
        }

        public IList<GroupMemberContract> GetGroupMembers(long groupId)
        {
            return m_groupManager.GetGroupMembers(groupId);
        }

        public IList<long> GetGroupMemberIds(long groupId)
        {
            return m_groupManager.GetGroupMemberIds(groupId);
        }
    }
}
//    {
//        private readonly ApplicationRepository m_applicationRepository;
//        private readonly AzureTableSynchronizedObjectDao m_azureTableSynchronizedObjectDao;
//        private readonly AzureTableTaskDao m_azureTableTaskDao;
//        private readonly EnterCodeGenerator m_enterCodeGenerator;
//        private readonly GroupRepository m_groupRepository;
//        private readonly InstitutionRepository m_institutionRepository;
//        private readonly int m_maxAttemptsToSave;
//        private readonly SynchronizedObjectRepository m_synchronizedObjectRepository;
//        private readonly TaskRepository m_taskRepository;
//        private readonly UserManager m_userManager;
//        private readonly UsersRepository m_usersRepository;

//        public MobileServiceManager(IKernel container, int maxAttemptsToSave)
//        {
//            m_usersRepository = container.Resolve<UsersRepository>();
//            m_synchronizedObjectRepository = container.Resolve<SynchronizedObjectRepository>();
//            m_institutionRepository = container.Resolve<InstitutionRepository>();
//            m_groupRepository = container.Resolve<GroupRepository>();
//            m_taskRepository = container.Resolve<TaskRepository>();
//            m_applicationRepository = container.Resolve<ApplicationRepository>();
//            m_azureTableTaskDao = container.Resolve<AzureTableTaskDao>();
//            m_azureTableSynchronizedObjectDao = container.Resolve<AzureTableSynchronizedObjectDao>();
//            m_userManager = container.Resolve<UserManager>();
//            m_enterCodeGenerator = container.Resolve<EnterCodeGenerator>();
//            m_maxAttemptsToSave = maxAttemptsToSave;
//        }

//        public void CreateInstitution(Institution institution)
//        {
//            var deInstitution = Mapper.Map<DE.Institution>(institution);
//            deInstitution.CreateTime = DateTime.UtcNow;
//            int attempt = 0;
//            while (attempt < m_maxAttemptsToSave)
//            {
//                try
//                {
//                    deInstitution.EnterCode = m_enterCodeGenerator.GenerateCode();
//                    m_institutionRepository.Create(deInstitution);
//                    return;
//                }
//                catch (CreateEntityFailedException)
//                {
//                    ++attempt;
//                }
//            }
//            //TODO throw exception or send fail message here
//        }

//        public InstitutionDetails GetInstitutionDetails(string institutionId)
//        {
//            DE.Institution institution = m_institutionRepository.LoadInstitutionWithDetails(long.Parse(institutionId));
//            return Mapper.Map<InstitutionDetails>(institution);
//        }

//        public void AddUserToInstitution(string enterCode, string userId)
//        {
//            DE.Institution institution = m_institutionRepository.FindByEnterCode(enterCode);
//            DE.User user = m_usersRepository.FindById(long.Parse(userId));
//            user.Institution = institution;
//            m_groupRepository.Update(user);
//        }


//        public void CreateUser(string authenticationProviderToken, AuthProvidersContract authenticationProvider, UserDetailContract userDetailContract)
//        {
//            m_userManager.CreateAccount(authenticationProviderToken, authenticationProvider, userDetailContract);
//        }

//        public LoginUserResponse LoginUser(UserLogin userLogin)
//        {
//            return m_userManager.Login(userLogin);
//        }


//        public UserDetails GetUserDetails(string userId)
//        {
//            DE.User user = m_usersRepository.LoadUserWithDetails(long.Parse(userId));
//            return Mapper.Map<UserDetails>(user);
//        }

//        public IEnumerable<TaskDetails> GetTasksByUser(string userId)
//        {
//            DE.User author = m_usersRepository.FindById(long.Parse(userId));
//            IList<DE.Task> tasks = m_taskRepository.LoadTasksWithDetailsByAuthor(author);
//            foreach (DE.Task task in tasks)
//            {
//                TaskEntity taskEntity = m_azureTableTaskDao.FindByRowAndPartitionKey(task.Id.ToString(), task.Application.Id.ToString());
//                if (taskEntity != null) task.Data = taskEntity.Data;
//            }
//            return Mapper.Map<IList<TaskDetails>>(tasks);
//        }

//        public IEnumerable<GroupDetail> GetGroupsByUser(string userId)
//        {
//            IList<DE.Group> groups = m_groupRepository.LoadGroupsWithDetailsByAuthorId(long.Parse(userId));
//            return Mapper.Map<IList<GroupDetail>>(groups);
//        }

//        public IEnumerable<GroupDetail> GetGroupListForUser(string userId)
//        {
//            var userWithGroup = m_usersRepository.LoadGroupsForUser(long.Parse(userId));
//            var memberOfGroups = userWithGroup.MemberOfGroups;
//            var ownedGroups = userWithGroup.CreatedGroups;

//            var result = new List<GroupDetail>();
//            result.AddRange(Mapper.Map<IEnumerable<GroupDetail>>(memberOfGroups));
//            result.AddRange(Mapper.Map<IEnumerable<OwnedGroupDetail>>(ownedGroups));
//            return result;
//        }

//        public IEnumerable<TaskDetails> GetTasksForApplication(string applicationId)
//        {
//            DE.Application application = m_applicationRepository.FindById(long.Parse(applicationId));
//            IList<DE.Task> tasks = m_taskRepository.LoadTasksWithDetailsByApplication(application);
//            foreach (DE.Task task in tasks) //TODO try to find some better way how to fill Data property
//            {
//                TaskEntity taskEntity = m_azureTableTaskDao.FindByRowAndPartitionKey(task.Id.ToString(), task.Application.Id.ToString());
//                if (taskEntity != null) task.Data = taskEntity.Data;
//            }
//            return Mapper.Map<IList<TaskDetails>>(tasks);
//        }

//        public void CreateTaskForApplication(string applicationId, string userId, Task apptask)
//        {
//            DE.Application application = m_applicationRepository.FindById(long.Parse(applicationId));
//            DE.User user = m_usersRepository.FindById(long.Parse(userId));
//            var deTask = Mapper.Map<DE.Task>(apptask);
//            deTask.Application = application;
//            deTask.Author = user;
//            deTask.CreateTime = DateTime.UtcNow;
//            object taskId = m_taskRepository.Create(deTask);
//            m_azureTableTaskDao.Create(new TaskEntity(taskId.ToString(), applicationId, apptask.Data));
//        }

//        public CreateGroupResponse CreateGroup(string userId, string groupName)
//        {
//            DE.User user = m_usersRepository.FindById(long.Parse(userId));
//            var deGroup = new DE.Group {Author = user, CreateTime = DateTime.UtcNow, Name = groupName, IsActive = true};
//            int attempt = 0;
//            while (attempt < m_maxAttemptsToSave)
//            {
//                try
//                {
//                    deGroup.EnterCode = m_enterCodeGenerator.GenerateCode();
//                    m_groupRepository.Create(deGroup);
//                    return new CreateGroupResponse {EnterCode = deGroup.EnterCode};
//                }
//                catch (CreateEntityFailedException)
//                {
//                    ++attempt;
//                }
//            }
//            return null; //TODO return exception here or fail message here
//        }

//        public void AssignTaskToGroup(string groupId, string taskId, string userId)
//        {
//            m_groupRepository.AssignTaskToGroup(groupId, taskId, userId);
//        }

//        public void AddUserToGroup(string enterCode, string userId)
//        {
//            DE.Group group = m_groupRepository.FindByEnterCode(enterCode);
//            DE.User user = m_usersRepository.FindById(long.Parse(userId));
//            group.Members.Add(user);
//            m_groupRepository.Update(group);
//        }

//        public GroupDetail GetGroupDetails(string groupId)
//        {
//            DE.Group group = m_groupRepository.LoadGroupWithDetails(long.Parse(groupId));
//            return Mapper.Map<GroupDetail>(group);
//        }

//        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string applicationId, string objectType, string since)
//        {
//            var group = m_groupRepository.FindById<DE.Group>(long.Parse(groupId)); //TODO fix not single transcation operation
//            DE.Application application = m_applicationRepository.FindById(long.Parse(applicationId));
//            DateTime sinceTime = DateTime.Parse(since);
//            IList<DE.SynchronizedObject> syncObjs = m_synchronizedObjectRepository.LoadSyncObjectsWithDetails(group, application, objectType,
//                sinceTime);
//            foreach (DE.SynchronizedObject syncObj in syncObjs) //TODO try to find some better way how to fill Data property
//            {
//                SynchronizedObjectEntity syncObjEntity = m_azureTableSynchronizedObjectDao.FindByRowAndPartitionKey(syncObj.Id.ToString(),
//                    syncObj.Group.Id.ToString());
//                if (syncObjEntity != null) syncObj.Data = syncObjEntity.Data;
//            }
//            return Mapper.Map<IList<SynchronizedObjectDetails>>(syncObjs);
//        }

//        public void CreateSynchronizedObject(string groupId, string applicationId, string userId, SynchronizedObject synchronizedObject)
//        {
//            DE.Application application = m_applicationRepository.FindById(long.Parse(applicationId)); //TODO fix not single transcation operation
//            DE.User user = m_usersRepository.FindById(long.Parse(userId));
//            var group = m_groupRepository.FindById<DE.Group>(long.Parse(userId));
//            var deSyncObject = Mapper.Map<DE.SynchronizedObject>(synchronizedObject);
//            deSyncObject.Application = application;
//            deSyncObject.Author = user;
//            deSyncObject.Group = group;
//            deSyncObject.CreateTime = DateTime.UtcNow;
//            object syncObjId = m_synchronizedObjectRepository.Create(deSyncObject);
//            m_azureTableSynchronizedObjectDao.Create(new SynchronizedObjectEntity(syncObjId.ToString(), groupId, synchronizedObject.Data));
//        }
//    }
//}