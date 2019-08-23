using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
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

        public AuthorizationManager(AuthenticationManager authenticationManager, PermissionRepository permissionRepository)
        {
            m_authenticationManager = authenticationManager;
            m_permissionRepository = permissionRepository;
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

        public void FilterProjectIdList(ref IList<long> projectIds)
        {
            if (projectIds == null || projectIds.Count == 0)
            {
                return;
            }

            IList<long> filtered;
            var user = m_authenticationManager.GetCurrentUser();

            if (user != null)
            {
                filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, projectIds);
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.FindGroupByExternalIdOrCreate(role.Id, role.Name);
                filtered = m_permissionRepository.GetFilteredBookIdListByGroupPermissions(group.Id, projectIds);
            }

            projectIds = filtered;
        }

        public void AuthorizeBook(long projectId)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user != null)
            {
                var filtered = m_permissionRepository.InvokeUnitOfWork(x =>
                    x.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> {projectId}));
                if (filtered == null || filtered.Count == 0)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UserBookAccessForbidden,
                        $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission on book with id '{projectId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(role.Id, role.Name));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x =>
                    x.GetFilteredBookIdListByGroupPermissions(group.Id, new List<long> {projectId}));

                if (filtered == null || filtered.Count == 0)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredUserBookAccessForbidden,
                        $"Unregistered user does not have permission on book with id '{projectId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
        }

        public void AuthorizeResource(long resourceId)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user != null)
            {
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserPermissions(user.Id, resourceId));

                if (filtered == null)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UserResourceAccessForbidden,
                        $"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission on book with resource with id '{resourceId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
            else
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(role.Id, role.Name));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserGroupPermissions(group.Id, resourceId));

                if (filtered == null)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.UnregisteredUserResourceAccessForbidden,
                        $"Unregistered user does not have permission on book with resource with id '{resourceId}'",
                        HttpStatusCode.Forbidden
                    );
                }
            }
        }
    }
}