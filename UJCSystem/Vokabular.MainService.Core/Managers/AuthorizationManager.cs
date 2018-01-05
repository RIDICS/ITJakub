using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

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

        public PermissionResult CheckUserCanAddNews()
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.Action));
            var newsPermissions = specialPermissions.OfType<NewsPermission>();
            if (!newsPermissions.Any(x => x.CanAddNews))
            {
                throw new UnauthorizedException(
                    string.Format("User with username '{0}' does not have permission to add news", user.UserName));
            }
            
            return new PermissionResult
            {
                UserId = user.Id,
            };
        }

        public void CheckUserCanManageFeedbacks()
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.Action));
            var feedbackPermissions = specialPermissions.OfType<FeedbackPermission>();
            if (!feedbackPermissions.Any(x => x.CanManageFeedbacks))
            {
                throw new UnauthorizedException(
                    string.Format("User with username '{0}' does not have permission to manage feedbacks", user.UserName));
            }
        }

        public void CheckUserCanManagePermissions()
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.Action));
            var managePermissionsPermissions = specialPermissions.OfType<ManagePermissionsPermission>();
            if (!managePermissionsPermissions.Any(x => x.CanManagePermissions))
            {
                throw new UnauthorizedException(
                    string.Format("User with username '{0}' does not have permission to manage permissions",
                        user.UserName));
            }
        }

        public PermissionResult CheckUserCanUploadBook()
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.Action));
            var uploadBookPermissions = specialPermissions.OfType<UploadBookPermission>();
            if (!uploadBookPermissions.Any(x => x.CanUploadBook))
            {
                throw new UnauthorizedException(
                    string.Format("User with username '{0}' does not have permission to upload books", user.UserName));
            }

            return new PermissionResult
            {
                UserId = user.Id,
            };
        }

        public void CheckUserCanViewCardFile(string cardFileId)
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.CardFile));
            var cardFilePermissions = specialPermissions.OfType<CardFilePermission>();
            if (!cardFilePermissions.Any(x => x.CanReadCardFile && x.CardFileId == cardFileId))
            {
                throw new UnauthorizedException(
                    string.Format("User with username '{0}' does not have permission to read cardfile with id '{1}'",
                        user.UserName, cardFileId));
            }
        }

        public void FilterCardFileList(ref IList<CardFileContract> cardFilesContracts)
        {
            var user = m_authenticationManager.GetCurrentUser(true);

            if (cardFilesContracts == null || cardFilesContracts.Count == 0)
            {
                return;
            }

            var specialPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByUserAndType(user.Id,
                SpecialPermissionCategorization.CardFile));
            var cardFileSpecialPermissions = specialPermissions.OfType<CardFilePermission>();
            var allowedCardFileIds = cardFileSpecialPermissions.Where(x => x.CanReadCardFile).Select(x => x.CardFileId);
            cardFilesContracts = cardFilesContracts.Where(x => allowedCardFileIds.Contains(x.Id)).ToList();
        }

        public void AuthorizeBook(long bookId)
        {
            var user = m_authenticationManager.GetCurrentUser(true);
            var filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> { bookId });

            if (filtered == null || filtered.Count == 0)
            {
                throw new UnauthorizedException($"User with username '{user.UserName}' does not have permission on book with id '{bookId}'");
            }
        }
    }
}
