using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.FileProcessing.DataContracts;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;
using AuthRoleContract = Vokabular.Authentication.DataContracts.RoleContract;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly PermissionConverter m_permissionConverter;

        public PermissionManager(PermissionRepository permissionRepository, CommunicationProvider communicationProvider,
            PermissionConverter permissionConverter)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_permissionConverter = permissionConverter;
        }

        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync().GetAwaiter().GetResult();
            return m_permissionConverter.Convert(permissions);
        }

        public List<PermissionFromAuthContract> GetAutoImportSpecialPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync().GetAwaiter().GetResult();

            var result = permissions.Where(x => x.Name.StartsWith(PermissionNames.AutoImport)).Select(p => new PermissionFromAuthContract
            {
                Id = p.Id,
                Name = p.Name,
                Roles = p.Roles.Select(r => new RoleFromAuthContract
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList()
            }).ToList();

            return result;
        }

        public List<SpecialPermissionContract> GetSpecialPermissionsForRole(int roleId)
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();

            var permissions = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult().Permissions;
            return m_permissionConverter.Convert(permissions);
        }

        public void AddSpecialPermissionsToRole(int roleId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var permissions = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult().Permissions;
            var permissionsId = permissions.Select(x => x.Id).ToList();
            foreach (var permissionToAdd in specialPermissionsIds)
            {
                if (!permissionsId.Contains(permissionToAdd))
                {
                    permissionsId.Add(permissionToAdd);
                }
            }

            client.AssignPermissionsToRoleAsync(roleId, permissionsId).GetAwaiter().GetResult();
        }

        public void RemoveSpecialPermissionsFromRole(int roleId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var permissions = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult().Permissions;
            var permissionsId = permissions.Select(x => x.Id).ToList();
            foreach (var permissionToRemove in specialPermissionsIds)
            {
                permissionsId.Remove(permissionToRemove);
            }

            client.AssignPermissionsToRoleAsync(roleId, permissionsId).GetAwaiter().GetResult();
        }

        public void AddBooksAndCategoriesToGroup(int roleId, IList<long> bookIds)
        {
            new AddProjectsToUserGroupWork(m_permissionRepository, roleId, bookIds).Execute();
        }

        public void RemoveBooksAndCategoriesFromGroup(int roleId, IList<long> bookIds)
        {
            new RemoveProjectsFromUserGroupWork(m_permissionRepository, roleId, bookIds).Execute();
        }
    }
}