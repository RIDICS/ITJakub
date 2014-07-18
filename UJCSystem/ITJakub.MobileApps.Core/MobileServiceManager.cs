using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using Group = ITJakub.MobileApps.DataContracts.Group;
using Institution = ITJakub.MobileApps.DataContracts.Institution;
using SynchronizedObject = ITJakub.MobileApps.DataContracts.SynchronizedObject;
using User = ITJakub.MobileApps.DataContracts.User;

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
        }

        private DataEntities.Database.Entities.User ConvertUser(User user)
        {
            return new DataEntities.Database.Entities.User() {FirstName = user.FirstName, LastName = user.LastName, Email = user.Email};
        }

        private User ConvertUser(DataEntities.Database.Entities.User user)
        {
            return new User(){Email = user.Email, FirstName = user.FirstName, LastName = user.LastName};
        }


        private DataEntities.Database.Entities.Group ConvertGroup(Group group)
        {
            return new DataEntities.Database.Entities.Group() { Users = group.Users.Select(ConvertUser).ToList()};
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            var institution = m_storageManager.FindInstitutionById(long.Parse(institutionId));
            return new InstitutionDetails()
            {
                Id= institution.Id,
                InstitutionBaseInfo = new Institution() {Name = institution.Name},
                GroupIds = institution.Groups.Select(group => group.Id).ToList(),
                UserIds = institution.Users.Select(user => user.Id).ToList()
            };
        }

        public void CreateUser(User user)
        {
            m_storageManager.CreateUser(ConvertUser(user));
        }

        public UserDetails GetUserDetails(string userId)
        {
            var user = m_storageManager.FindUserById(long.Parse(userId));
            return new UserDetails()
            {
                Id = user.Id,
                User = new User() {FirstName = user.FirstName, LastName = user.LastName, Email = user.Email},
                Role = user.Role.Name,
                GroupIds = user.Groups.Select(group => group.Id).ToList(),
                TaskIds =  user.Tasks.Select(task => task.Id).ToList(),
            };
        }

        public IEnumerable<AppTaskDetails> GetTasksForApplication(string applicationId)
        {
            var application = m_storageManager.FindApplicationById(long.Parse(applicationId));
            throw new System.NotImplementedException();
        }

        public void CreateTaskForApplication(string applicationId, AppTask appTask)
        {
            var application = m_storageManager.FindApplicationById(long.Parse(applicationId));
            m_storageManager.CreateTask(application,appTask.Data);
        }

        public void CreateGroup(string institutionId, Group group)
        {
            var institution = m_storageManager.FindInstitutionById(long.Parse(institutionId));
            var task = m_storageManager.FindTaskById(group.TaskId);
            m_storageManager.CreateGroup(institution, task, ConvertGroup(group));
        }


        public GroupDetails GetGroupDetails(string groupId)
        {
            var group = m_storageManager.FindGroupById(long.Parse(groupId));
            return new GroupDetails()
            {
                Id = group.Id,
                CreateTime = group.CreateTime,
                AuthorId = group.Author.Id,
                Group = new Group(){ TaskId = group.Task.Id, Users = group.Users.Select(ConvertUser).ToList() }

            };
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