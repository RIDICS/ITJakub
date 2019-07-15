using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ITJakub.FileProcessing.DataContracts;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using AuthPermissionContract = Ridics.Authentication.DataContracts.PermissionContract;


namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        
        public PermissionManager(PermissionRepository permissionRepository, CommunicationProvider communicationProvider)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
        }

        public List<PermissionFromAuthContract> GetAutoImportSpecialPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync().GetAwaiter().GetResult();

            var result = permissions.Where(x => x.Name.StartsWith(VokabularPermissionNames.AutoImport)).Select(p => new PermissionFromAuthContract
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

        public List<PermissionContract> GetAllPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync().GetAwaiter().GetResult();
            return Mapper.Map<List<PermissionContract>>(permissions);
        }

        public PagedResultList<PermissionContract> GetPermissions(int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var result = client.HttpClient.GetListAsync<AuthPermissionContract>(startValue, countValue, filterByName).GetAwaiter().GetResult();
            var permissionContracts = Mapper.Map<List<PermissionContract>>(result.Items);

            return new PagedResultList<PermissionContract>
            {
                List = permissionContracts,
                TotalCount = result.ItemsCount
            };
        }
    }
}