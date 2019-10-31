using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.FileProcessing.DataContracts;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly ProjectPermissionConverter m_permissionConverter;
        private readonly IMapper m_mapper;

        public PermissionManager(PermissionRepository permissionRepository, CommunicationProvider communicationProvider,
            DefaultUserProvider defaultUserProvider, ProjectPermissionConverter permissionConverter, IMapper mapper)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_defaultUserProvider = defaultUserProvider;
            m_permissionConverter = permissionConverter;
            m_mapper = mapper;
        }

        public List<PermissionFromAuthContract> GetAutoImportSpecialPermissions()
        {
            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var permissions = client.GetAllPermissionsAsync(VokabularPermissionNames.AutoImport).GetAwaiter().GetResult();

            var result = permissions.Where(x => x.Name.StartsWith(VokabularPermissionNames.AutoImport)).Select(p =>
                new PermissionFromAuthContract
                {
                    Id = p.Id,
                    Name = p.Name,
                    RoleExternalIds = null, // Loaded by additional requests
                }).ToList();

            foreach (var permissionFromAuthContract in result)
            {
                permissionFromAuthContract.RoleExternalIds =
                    client.GetRoleIdsByPermissionAsync(permissionFromAuthContract.Id).GetAwaiter().GetResult();
            }

            return result;
        }

        public void AddSpecialPermissionsToRole(int roleId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var userGroup = m_permissionRepository.InvokeUnitOfWork(x => x.FindById<RoleUserGroup>(roleId));

            var roleClient = m_communicationProvider.GetAuthRoleApiClient();
            var permissionClient = m_communicationProvider.GetAuthPermissionApiClient();
            var defaultUnregisteredRole = m_defaultUserProvider.GetDefaultUnregisteredRole();
            var permissions = roleClient.GetRoleAsync(userGroup.ExternalId, true).GetAwaiter().GetResult().Permissions;
            var permissionsId = permissions.Select(x => x.Id).ToList();
            foreach (var permissionToAdd in specialPermissionsIds)
            {
                if (!permissionsId.Contains(permissionToAdd))
                {
                    var permission = permissionClient.GetPermissionAsync(permissionToAdd).GetAwaiter().GetResult();

                    // Deny assigning special permissions to Unregistered user (not logged in)
                    if (defaultUnregisteredRole.Id == userGroup.ExternalId && !permission.Name.Contains(VokabularPermissionNames.AutoImport) &&
                        !permission.Name.Contains(VokabularPermissionNames.CardFile))
                    {
                        throw new ArgumentException($"Special permissions cannot be added to the default role {defaultUnregisteredRole.Name}.");
                    }
                    permissionsId.Add(permissionToAdd);
                }
            }

            roleClient.AssignPermissionsToRoleAsync(userGroup.ExternalId, permissionsId).GetAwaiter().GetResult();
        }

        public void RemoveSpecialPermissionsFromRole(int roleId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            var userGroup = m_permissionRepository.InvokeUnitOfWork(x => x.FindById<RoleUserGroup>(roleId));

            var client = m_communicationProvider.GetAuthRoleApiClient();
            var permissions = client.GetRoleAsync(userGroup.ExternalId, true).GetAwaiter().GetResult().Permissions;
            var permissionsId = permissions.Select(x => x.Id).ToList();
            foreach (var permissionToRemove in specialPermissionsIds)
            {
                permissionsId.Remove(permissionToRemove);
            }

            client.AssignPermissionsToRoleAsync(userGroup.ExternalId, permissionsId).GetAwaiter().GetResult();
        }

        public void UpdateOrAddBooksToRole(int roleId, IList<long> bookIds, PermissionDataContract data)
        {
            var permissionFlags = m_permissionConverter.GetFlags(data);

            //new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new UpdateOrAddProjectsToRoleWork(m_permissionRepository, roleId, bookIds, permissionFlags).Execute();
        }

        public void RemoveBooksFromRole(int roleId, IList<long> bookIds)
        {
            //new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new RemoveProjectsFromRoleWork(m_permissionRepository, roleId, bookIds).Execute();
        }

        public PermissionDataContract GetPermissionsForRoleAndBook(int roleId, long bookId)
        {
            //new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            var dbResult = m_permissionRepository.InvokeUnitOfWork(x => x.FindPermissionByBookAndGroup(bookId, roleId));
            var result = m_mapper.Map<PermissionDataContract>(dbResult);
            return result;
        }

        public PagedResultList<SpecialPermissionContract> GetPermissions(int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var client = m_communicationProvider.GetAuthPermissionApiClient();

            var result = client.GetPermissionListAsync(startValue, countValue, filterByName).GetAwaiter()
                .GetResult();
            var permissionContracts = m_mapper.Map<List<SpecialPermissionContract>>(result.Items);

            return new PagedResultList<SpecialPermissionContract>
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