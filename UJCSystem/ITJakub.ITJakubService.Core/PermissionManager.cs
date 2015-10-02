using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class PermissionManager
    {
        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly AuthorizationManager m_authorizationManager;


        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public PermissionManager(UserRepository userRepository, PermissionRepository permissionRepository, BookRepository bookRepository, CategoryRepository categoryRepository, AuthorizationManager authorizationManager)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_authorizationManager = authorizationManager;
            m_bookRepository = bookRepository;
            m_categoryRepository = categoryRepository;
        }

        public List<GroupContract> GetGroupsByUser(int userId)
        {
            User user = m_userRepository.FindById(userId);

            if (user == null)
            {
                string message = string.Format("Cannot locate user with id '{0}'", userId);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var groups = m_permissionRepository.GetGroupsByUser(user.Id);
            return Mapper.Map<List<GroupContract>>(groups);
        }

        public List<UserContract> GetUsersByGroup(int groupId)
        {
            var users = m_permissionRepository.GetUsersByGroup(groupId);
            return Mapper.Map<List<UserContract>>(users);
        }

        public GroupDetailContract CreateGroup(string groupName, string description)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var user = m_authorizationManager.GetCurrentUser();

            var group = new Group
            {
                Name = groupName,
                Description = description,
                CreateTime = DateTime.UtcNow,
                CreatedBy = user
            };

            var groupId = m_permissionRepository.CreateGroup(group);
            return GetGroupDetail(groupId);
        }

        public GroupDetailContract GetGroupDetail(int groupId)
        {
            var group = m_permissionRepository.FindGroupById(groupId);
            if (group == null) return null;
            var groupContract = Mapper.Map<GroupDetailContract>(group);
            return groupContract;
        }

        public void DeleteGroup(int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var group = m_permissionRepository.FindGroupById(groupId);
            m_permissionRepository.Delete(group);
        }

        public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var group = m_permissionRepository.FindGroupById(groupId);

            var allBookIds = new List<long>();

            if (categoryIds != null && categoryIds.Count > 0)
            {
                var bookIdsFromCategories = m_categoryRepository.GetBookIdsFromCategory(categoryIds);
                allBookIds.AddRange(bookIdsFromCategories);
            }

            if (bookIds != null)
            {
                allBookIds.AddRange(bookIds);
            }

            var books = m_bookRepository.GetBooksById(allBookIds);
            var permissionsList = new List<Permission>();

            foreach (var book in books)
            {
                    permissionsList.Add(new Permission
                    {
                        Book = book,
                        Group = group
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
                        m_log.WarnFormat("Cannot save permission for group witd id '{0}' on book with id '{1}' for reason '{2}'", permission.Group.Id, permission.Book.Id, ex.InnerException);
                }

            }
        }

        public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var allBookIds = new List<long>();

            if (categoryIds != null && categoryIds.Count > 0)
            {
                var bookIdsFromCategories = m_categoryRepository.GetBookIdsFromCategory(categoryIds);
                allBookIds.AddRange(bookIdsFromCategories);
            }

            if (bookIds != null)
            {
                allBookIds.AddRange(bookIds);
            }

            var permissions = m_permissionRepository.FindPermissionsByGroupAndBooks(groupId, allBookIds);
            m_permissionRepository.DeletePermissions(permissions);
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var group = m_permissionRepository.FindGroupById(groupId);
            var user = m_userRepository.FindById(userId);

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

        public void AddUserToGroup(int userId, int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var group = m_permissionRepository.FindGroupById(groupId);
            var user = m_userRepository.FindById(userId);

            if (group.Users == null)
            {
                group.Users = new List<User>();
            }

            group.Users.Add(user);
            m_permissionRepository.Save(group);
        }

        public CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var books = m_categoryRepository.FindChildBookVersionsInCategory(categoryId);
            m_authorizationManager.FilterBooksByGroup(groupId, ref books);
            var categories = m_categoryRepository.FindChildCategoriesInCategory(categoryId);

            return new CategoryContentContract
            {
                Books = Mapper.Map<IList<BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public CategoryContentContract GetAllCategoryContent(int categoryId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var books = m_categoryRepository.FindChildBookVersionsInCategory(categoryId);
            var categories = m_categoryRepository.FindChildCategoriesInCategory(categoryId);

            return new CategoryContentContract
            {
                Books = Mapper.Map<IList<BookContract>>(books),
                Categories = Mapper.Map<IList<CategoryContract>>(categories)
            };
        }

        public IList<SpecialPermissionContract> GetSpecialPermissionsForUser(int userId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.GetSpecialPermissionsByUser(userId);
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public IList<SpecialPermissionContract> GetSpecialPermissions()
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.GetSpecialPermissions();
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }
        public IList<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.GetSpecialPermissionsByGroup(groupId);
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByIds(specialPermissionsIds);
            var group = m_permissionRepository.FindGroupWithSpecialPermissionsById(groupId);

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

        public void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByIds(specialPermissionsIds);
            var group = m_permissionRepository.FindGroupWithSpecialPermissionsById(groupId);

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
}