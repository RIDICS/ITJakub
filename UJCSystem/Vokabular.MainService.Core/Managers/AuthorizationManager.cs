using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthorizationManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AuthenticationManager m_authenticationManager;
        private readonly PermissionRepository m_permissionRepository;
        private readonly ProjectPermissionConverter m_projectPermissionConverter;

        public AuthorizationManager(AuthenticationManager authenticationManager, PermissionRepository permissionRepository, ProjectPermissionConverter projectPermissionConverter)
        {
            m_authenticationManager = authenticationManager;
            m_permissionRepository = permissionRepository;
            m_projectPermissionConverter = projectPermissionConverter;
        }

        public int GetCurrentUserId()
        {
            return m_authenticationManager.GetCurrentUser(true).Id;
        }

        public void AddAuthorizationCriteria(IList<SearchCriteriaContract> searchCriteriaConjuction)
        {
            var user = m_authenticationManager.GetCurrentUser(true);

            if (searchCriteriaConjuction.Any(x => x.Key == CriteriaKey.Authorization))
            {
                if (m_log.IsWarnEnabled)
                    m_log.WarnFormat("Recieved authorizeCriteria in request from user with id '{0}'", user.Id);

                throw new MainServiceException(MainServiceErrorCode.UnallowedAuthorizationCriteria,
                    "Search criteria contains unallowed Authorization criteria. Authorization criteria is generated automatically.");
            }

            var authorizationCriteria = new AuthorizationCriteriaContract {UserId = user.Id};
            searchCriteriaConjuction.Add(authorizationCriteria);
        }

        public void CheckUserCanViewCardFile(string cardFileId)
        {
            var currentUserPermissions = m_authenticationManager.GetCurrentUserPermissions(true);
            if (currentUserPermissions.All(x => x.Value != VokabularPermissionNames.CardFile + cardFileId))
            {
                var user = m_authenticationManager.GetCurrentUser();

                if (user == null)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredCardFileAccessForbidden,
                        $"Unregistered user does not have permission to read cardfile with id '{cardFileId}'",
                        HttpStatusCode.Forbidden
                    );
                }

                throw new MainServiceException(
                    MainServiceErrorCode.UserCardFileAccessForbidden,
                    $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission to read cardfile with id '{cardFileId}'",
                    HttpStatusCode.Forbidden
                );
            }
        }

        public void FilterCardFileList(ref IList<CardFileContract> cardFilesContracts)
        {
            if (cardFilesContracts == null || cardFilesContracts.Count == 0)
            {
                return;
            }

            var currentUserPermissions = m_authenticationManager.GetCurrentUserPermissions(true);
            cardFilesContracts = cardFilesContracts
                .Where(x => currentUserPermissions.Any(y => y.Value == VokabularPermissionNames.CardFile + x.Id))
                .ToList();
        }

        public void FilterProjectIdList(ref IList<long> projectIds, PermissionFlag permission)
        {
            if (projectIds == null || projectIds.Count == 0)
            {
                return;
            }

            IList<long> filtered;
            var user = m_authenticationManager.GetCurrentUser();

            if (user != null)
            {
                filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, projectIds, permission);
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.FindGroupByExternalIdOrCreate(role.Id, role.Name);
                filtered = m_permissionRepository.GetFilteredBookIdListByGroupPermissions(group.Id, projectIds, permission);
            }

            projectIds = filtered;
        }

        public void AuthorizeBook(long projectId, PermissionFlag permission)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user != null)
            {
                var filtered = m_permissionRepository.InvokeUnitOfWork(x =>
                    x.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> {projectId}, permission));
                if (filtered == null || filtered.Count == 0)
                {
                    string errorCode;
                    switch (permission)
                    {
                        case PermissionFlag.ShowPublished:
                            errorCode = MainServiceErrorCode.UserBookReadForbidden;
                            break;
                        case PermissionFlag.ReadProject:
                            errorCode = MainServiceErrorCode.UserBookReadProjectForbidden;
                            break;
                        case PermissionFlag.EditProject:
                            errorCode = MainServiceErrorCode.UserBookEditProjectForbidden;
                            break;
                        case PermissionFlag.AdminProject:
                            errorCode = MainServiceErrorCode.UserBookAdminProjectForbidden;
                            break;
                        default:
                            errorCode = MainServiceErrorCode.UserBookAccessForbidden;
                            break;
                    }

                    throw new MainServiceException(
                        errorCode,
                        $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission {permission} on book with id '{projectId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(role.Id, role.Name));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x =>
                    x.GetFilteredBookIdListByGroupPermissions(group.Id, new List<long> {projectId}, permission));

                if (filtered == null || filtered.Count == 0)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredUserBookAccessForbidden,
                        $"Unregistered user does not have permission {permission} on book with id '{projectId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
        }

        public void AuthorizeBookOrPermission(long projectId, PermissionFlag permission, string permissionName)
        {
            var userPermissions = m_authenticationManager.GetCurrentUserPermissions(false);
            if (userPermissions.Any(x => x.Value == permissionName))
            {
                return;
            }

            AuthorizeBook(projectId, permission);
        }

        public void AuthorizeSnapshot(long snapshotId, PermissionFlag permission = PermissionFlag.ShowPublished)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user != null)
            {
                var permissions = m_permissionRepository.InvokeUnitOfWork(x => x.FindPermissionsForSnapshotByUserId(snapshotId, user.Id));
                if (permissions == null || !permissions.Any(x => x.Flags.HasFlag(permission)))
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UserBookReadForbidden,
                        $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission {permission} on book with Snapshot ID '{snapshotId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(role.Id, role.Name));
                var dbPermission = m_permissionRepository.InvokeUnitOfWork(x => x.FindPermissionForSnapshotByGroupId(snapshotId, group.Id));

                if (dbPermission == null || !dbPermission.Flags.HasFlag(permission))
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredUserBookAccessForbidden,
                        $"Unregistered user does not have permission {permission} on book with Snapshot ID '{snapshotId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
        }

        public void AuthorizeResource(long resourceId, PermissionFlag permission = PermissionFlag.ShowPublished)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user != null)
            {
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserPermissions(user.Id, resourceId, permission));

                if (filtered == null)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UserResourceAccessForbidden,
                        $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission {permission} on book with resource with id '{resourceId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(role.Id, role.Name));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserGroupPermissions(group.Id, resourceId, permission));

                if (filtered == null)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredUserResourceAccessForbidden,
                        $"Unregistered user does not have permission {permission} on book with resource with id '{resourceId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
        }
        
        public PermissionDataContract GetCurrentUserProjectPermissions(long projectId)
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            if (user == null)
            {
                return new PermissionDataContract();
            }

            var permissions = m_permissionRepository.InvokeUnitOfWork(x => x.FindPermissionsByUserAndBook(user.Id, projectId));
            var result = m_projectPermissionConverter.GetAggregatedPermissions(permissions);
            
            return result;
        }
    }
}