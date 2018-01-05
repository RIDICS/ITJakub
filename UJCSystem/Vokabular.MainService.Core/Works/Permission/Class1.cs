using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class AddSpecialPermissionsToGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<int> m_specialPermissionsIds;

        public AddSpecialPermissionsToGroupWork(PermissionRepository permissionRepository, int groupId, IList<int> specialPermissionsIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
            m_specialPermissionsIds = specialPermissionsIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByIds(m_specialPermissionsIds);
            var group = m_permissionRepository.FindGroupWithSpecialPermissionsById(m_groupId);

            if (group.SpecialPermissions == null)
            {
                group.SpecialPermissions = new List<SpecialPermission>();
            }

            var missingSpecialPermissions = specialPermissions.Where(x => !group.SpecialPermissions.Contains(x));

            foreach (var specialPermission in missingSpecialPermissions)
            {
                group.SpecialPermissions.Add(specialPermission);
            }

            m_permissionRepository.Save(group);
        }
    }

    public class RemoveSpecialPermissionsFromGroupWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<int> m_specialPermissionsIds;

        public RemoveSpecialPermissionsFromGroupWork(PermissionRepository permissionRepository, int groupId, IList<int> specialPermissionsIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
            m_specialPermissionsIds = specialPermissionsIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByIds(m_specialPermissionsIds);
            var group = m_permissionRepository.FindGroupWithSpecialPermissionsById(m_groupId);

            if (group.SpecialPermissions == null)
            {
                if (m_log.IsWarnEnabled)
                {
                    string message = string.Format("Cannot remove special permissions from group with id '{0}'. Group special permissions are empty.", group.Id);
                    m_log.Warn(message);
                }
                return;
            }

            foreach (var specialPermission in specialPermissions)
            {
                group.SpecialPermissions.Remove(specialPermission);
            }

            m_permissionRepository.Save(group);
        }
    }

    public class AddBooksAndCategoriesToGroupWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<long> m_bookIds;

        public AddBooksAndCategoriesToGroupWork(PermissionRepository permissionRepository, int groupId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
            m_bookIds = bookIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupById(m_groupId);

            var allBookIds = new List<long>();

            //if (categoryIds != null && categoryIds.Count > 0)
            //{
            //    var bookIdsFromCategories = m_categoryRepository.GetBookIdsFromCategory(categoryIds);
            //    allBookIds.AddRange(bookIdsFromCategories);
            //}

            if (m_bookIds != null)
            {
                allBookIds.AddRange(m_bookIds);
            }

            var permissionsList = new List<DataEntities.Database.Entities.Permission>();

            foreach (var bookId in allBookIds)
            {
                var book = m_permissionRepository.Load<Project>(bookId);
                permissionsList.Add(new DataEntities.Database.Entities.Permission
                {
                    Project = book,
                    UserGroup = group
                });
            }

            foreach (var permission in permissionsList)
            {
                try
                {
                    m_permissionRepository.CreatePermission(permission);
                }
                catch (Exception ex)
                {
                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Cannot save permission for group witd id '{0}' on book with id '{1}' for reason '{2}'", permission.UserGroup.Id, permission.Project.Id, ex.InnerException);
                }
            }
        }
    }

    public class RemoveBooksAndCategoriesFromGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<long> m_bookIds;

        public RemoveBooksAndCategoriesFromGroupWork(PermissionRepository permissionRepository, int groupId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
            m_bookIds = bookIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var allBookIds = new List<long>();

            //if (categoryIds != null && categoryIds.Count > 0)
            //{
            //    var bookIdsFromCategories = m_categoryRepository.GetBookIdsFromCategory(categoryIds);
            //    allBookIds.AddRange(bookIdsFromCategories);
            //}

            if (m_bookIds != null)
            {
                allBookIds.AddRange(m_bookIds);
            }

            var permissions = m_permissionRepository.FindPermissionsByGroupAndBooks(m_groupId, allBookIds);
            m_permissionRepository.DeletePermissions(permissions);
        }
    }





    public class CreateGroupWork : UnitOfWorkBase<int>
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly string m_groupName;
        private readonly string m_description;
        private readonly int m_userId;

        public CreateGroupWork(PermissionRepository permissionRepository, string groupName, string description, int userId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupName = groupName;
            m_description = description;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_permissionRepository.Load<User>(m_userId);

            var group = new UserGroup
            {
                Name = m_groupName,
                Description = m_description,
                CreateTime = now,
                CreatedBy = user
            };

            var groupId = m_permissionRepository.CreateGroup(group);
            return groupId;
        }
    }
    public class DeleteGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;

        public DeleteGroupWork(PermissionRepository permissionRepository, int groupId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindById<Group>(m_groupId);
            m_permissionRepository.Delete(group);
        }
    }
    public class AddUserToGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_userId;
        private readonly int m_groupId;

        public AddUserToGroupWork(PermissionRepository permissionRepository, int userId, int groupId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_userId = userId;
            m_groupId = groupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupById(m_groupId);
            var user = m_permissionRepository.Load<User>(m_userId);

            //TODO switch logic: remove group from user (fetch lower amount of data)

            if (group.Users == null)
            {
                group.Users = new List<User>();
            }

            group.Users.Add(user);
            m_permissionRepository.Save(group);
        }
    }
    public class RemoveUserFromGroupWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_userId;
        private readonly int m_groupId;

        public RemoveUserFromGroupWork(PermissionRepository permissionRepository, int userId, int groupId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_userId = userId;
            m_groupId = groupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupById(m_groupId);
            var user = m_permissionRepository.Load<User>(m_userId);

            //TODO switch logic: remove group from user (fetch lower amount of data)

            if (group.Users == null)
            {
                if (m_log.IsWarnEnabled)
                {
                    string message = string.Format("Cannot remove user with id '{0}' from group with id '{1}'. Group is empty.", user.Id, group.Id);
                    m_log.Warn(message);
                }
                return;
            }

            group.Users.Remove(user);
            m_permissionRepository.Save(group);
        }
    }
}
