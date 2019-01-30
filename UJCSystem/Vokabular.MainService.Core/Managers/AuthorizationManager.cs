﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

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

                throw new ArgumentException("Search criteria contains unallowed Authorization criteria. Authorization criteria is generated automatically.");
            }

            var authorizationCriteria = new AuthorizationCriteriaContract { UserId = user.Id };
            searchCriteriaConjuction.Add(authorizationCriteria);
        }

        public void CheckUserCanViewCardFile(string cardFileId)
        {
            var currentUserPermissions = m_authenticationManager.GetCurrentUserPermissions(true);
            if (currentUserPermissions.All(x => x.Value != PermissionNames.CardFile + cardFileId))
            {
                try
                {
                    var user = m_authenticationManager.GetCurrentUser();

                    throw new UnauthorizedException(
                        $"User with id '{user.Id}' (external id '{user.ExternalId}')  does not have permission to read cardfile with id '{cardFileId}'");
                }
                catch (AuthenticationException)
                {
                    throw new UnauthorizedException(
                        $"Unregistered user does not have permission to read cardfile with id '{cardFileId}'");
                }
            }
        }

        public void FilterCardFileList(ref IList<CardFileContract> cardFilesContracts)
        {
            if (cardFilesContracts == null || cardFilesContracts.Count == 0)
            {
                return;
            }

            var currentUserPermissions = m_authenticationManager.GetCurrentUserPermissions(true);
            cardFilesContracts = cardFilesContracts.Where(x => currentUserPermissions.Any(y => y.Value == PermissionNames.CardFile + x.Id)).ToList();
        }

        public void FilterProjectIdList(ref IList<long> projectIds)
        {
            if (projectIds == null || projectIds.Count == 0)
            {
                return;
            }

            IList<long> filtered;
            try
            {
                var user = m_authenticationManager.GetCurrentUser();
                filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, projectIds);
            }
            catch (AuthenticationException)
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.FindGroupByExternalId(role.Id);
                filtered = m_permissionRepository.GetFilteredBookIdListByGroupPermissions(group.Id, projectIds);                
            }

            projectIds = filtered;
        }

        public void AuthorizeBook(long projectId)
        {
            try
            {
                var user = m_authenticationManager.GetCurrentUser();
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> { projectId }));
                if (filtered == null || filtered.Count == 0)
                {
                    throw new UnauthorizedException($"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission on book with id '{projectId}'");
                }
            }
            catch (AuthenticationException)
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalId(role.Id));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetFilteredBookIdListByGroupPermissions(group.Id, new List<long> { projectId }));

                if (filtered == null || filtered.Count == 0)
                {
                    throw new UnauthorizedException($"Unregistered user does not have permission on book with id '{projectId}'");
                }
            }
        }

        public void AuthorizeResource(long resourceId)
        {
            try
            {
                var user = m_authenticationManager.GetCurrentUser();
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserPermissions(user.Id, resourceId));

                if (filtered == null)
                {
                    throw new UnauthorizedException($"User with id '{user.Id}' (external id '{user.ExternalId}') does not have permission on book with resource with id '{resourceId}'");
                }
            }
            catch (AuthenticationException)
            {
                var role = m_authenticationManager.GetUnregisteredRole();
                var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalId(role.Id));
                var filtered = m_permissionRepository.InvokeUnitOfWork(x => x.GetResourceByUserGroupPermissions(group.Id, resourceId));

                if (filtered == null)
                {
                    throw new UnauthorizedException($"Unregistered user does not have permission on book with resource with id '{resourceId}'");
                }
            }
        }
    }
}
