using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.FileProcessing.DataContracts;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using AuthPermissionContract = Ridics.Authentication.DataContracts.PermissionContract;
using PermissionContract = Vokabular.MainService.DataContracts.Contracts.Permission.PermissionContract;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly DefaultUserProvider m_defaultUserProvider;

        public PermissionManager(PermissionRepository permissionRepository, CommunicationProvider communicationProvider,
            DefaultUserProvider defaultUserProvider)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_defaultUserProvider = defaultUserProvider;
        }

        public List<PermissionFromAuthContract> GetAutoImportSpecialPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync().GetAwaiter().GetResult();

            var result = permissions.Where(x => x.Name.StartsWith(VokabularPermissionNames.AutoImport)).Select(p =>
                new PermissionFromAuthContract
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
            var defaultUnregisteredRole = m_defaultUserProvider.GetDefaultUnregisteredRole();
            var permissions = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult().Permissions;
            var permissionsId = permissions.Select(x => x.Id).ToList();
            foreach (var permissionToAdd in specialPermissionsIds)
            {
                if (!permissionsId.Contains(permissionToAdd))
                {
                    var permission = client.HttpClient.GetItemAsync<AuthPermissionContract>(permissionToAdd).GetAwaiter().GetResult();

                    // Deny assigning special permissions to Unregistered user (not logged in)
                    if (defaultUnregisteredRole.Id == roleId && !permission.Name.Contains(VokabularPermissionNames.AutoImport) &&
                        !permission.Name.Contains(VokabularPermissionNames.CardFile))
                    {
                        throw new ArgumentException($"Special permissions cannot be added to the default role {defaultUnregisteredRole.Name}.");
                    }
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

        public void AddBooksToRole(int roleId, IList<long> bookIds)
        {
            new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new AddProjectsToRoleWork(m_permissionRepository, roleId, bookIds).Execute();
        }

        public void RemoveBooksFromRole(int roleId, IList<long> bookIds)
        {
            new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new RemoveProjectsFromRoleWork(m_permissionRepository, roleId, bookIds).Execute();
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

            var result = client.HttpClient.GetListAsync<AuthPermissionContract>(startValue, countValue, filterByName).GetAwaiter()
                .GetResult();
            var permissionContracts = Mapper.Map<List<PermissionContract>>(result.Items);

            return new PagedResultList<PermissionContract>
            {
                List = permissionContracts,
                TotalCount = result.ItemsCount
            };
        }

        public void EnsureAuthServiceHasRequiredPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var request = new EnsurePermissionsContract
            {
                NewAssignToRoleName = DefaultValues.RoleForNewPermissions,
                Permissions = DefaultValues.RequiredPermissionsWithDescription.Select(x => new PermissionContractBase
                {
                    Name = x.Item1,
                    Description = x.Item2,
                }).ToList()
            };

            client.EnsurePermissionsExistAsync(request).GetAwaiter().GetResult();
        }
    }
}