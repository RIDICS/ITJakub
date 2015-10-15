using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Searching.Criteria;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class AuthorizationManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly PermissionRepository m_permissionRepository;        
        private readonly UserManager m_userManager;

        public AuthorizationManager(UserManager userManager, UserRepository userRepository, PermissionRepository permissionRepository)
        {            
            m_userManager = userManager;
            m_permissionRepository = permissionRepository;
        }
  

        public void CheckUserCanAddNews()
        {
            var user = m_userManager.GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var newsPermissions = specialPermissions.OfType<NewsPermission>();
            if (!newsPermissions.Any(x => x.CanAddNews))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to add news", user.UserName));
            }

        }

        public void CheckUserCanManageFeedbacks()
        {
            var user = m_userManager.GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var feedbackPermissions = specialPermissions.OfType<FeedbackPermission>();
            if (!feedbackPermissions.Any(x => x.CanManageFeedbacks))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to manage feedbacks", user.UserName));
            }

        } 

        public void CheckUserCanManagePermissions()
        {
            var user = m_userManager.GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var managePermissionsPermissions = specialPermissions.OfType<ManagePermissionsPermission>();
            if (!managePermissionsPermissions.Any(x => x.CanManagePermissions))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to manage permissions", user.UserName));
            }
        } 

        public void CheckUserCanUploadBook()
        {
            return; //TODO HACK
            var user = m_userManager.GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var uploadBookPermissions = specialPermissions.OfType<UploadBookPermission>();
            if (!uploadBookPermissions.Any(x => x.CanUploadBook))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to upload books", user.UserName));
            }
        } 

        public void AuthorizeBook(string bookXmlId)
        {
            var user = m_userManager.GetCurrentUser();
            var filtered = m_permissionRepository.GetFilteredBookXmlIdListByUserPermissions(user.Id, new List<string> {bookXmlId});
            if (filtered == null)
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission on book with xmlId '{1}'", user.UserName,
                    bookXmlId));
            }
        }

        public void AuthorizeBook(long bookId)
        {
            var user = m_userManager.GetCurrentUser();
            var filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> {bookId});
            if (filtered == null)
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission on book with id '{1}'", user.UserName,
                    bookId));
            }
        }

        public void FilterBooksByCurrentUser(ref IList<BookVersion> books)
        {
            var user = m_userManager.GetCurrentUser();

            if (books == null || books.Count == 0)
            {
                return;
            }

            var bookIds = books.Select(x => x.Id).ToList();
            var filteredBookIds = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, bookIds);
            var filteredBooks = books.Where(x => filteredBookIds.Contains(x.Id)).ToList();
            books = filteredBooks;
        }

        public void FilterBooksByGroup(int groupId, ref IList<BookVersion> bookVersions)
        {
            if (bookVersions == null || bookVersions.Count == 0)
            {
                return;
            }

            var bookIds = bookVersions.Select(x => x.Book.Id).ToList();
            var filteredBookIds = m_permissionRepository.GetFilteredBookIdListByGroupPermissions(groupId, bookIds);
            var filteredBookVersions = bookVersions.Where(x => filteredBookIds.Contains(x.Book.Id)).ToList();
            bookVersions = filteredBookVersions;
        }

        public void AuthorizeCriteria(List<SearchCriteriaContract> searchCriteriaContracts)
        {
            var user = m_userManager.GetCurrentUser();
            var authCriterias = searchCriteriaContracts.OfType<AuthorizationCriteriaContract>().ToList();
            if (authCriterias.Count != 0)
            {
                if (m_log.IsWarnEnabled)
                    m_log.WarnFormat("Recieved authorizeCriteria in request from user with id '{0}'", user.Id);

                foreach (var authCriteria in authCriterias)
                {
                    searchCriteriaContracts.Remove(authCriteria);
                }
            }

            var authorizationCriteria = new AuthorizationCriteriaContract {UserId = user.Id};

            searchCriteriaContracts.Add(authorizationCriteria);
        }

        public void FilterBookIdList(ref IList<long> bookIds)
        {
            var user = m_userManager.GetCurrentUser();

            if (bookIds == null || bookIds.Count == 0)
            {
                return;
            }

            var filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, bookIds);
            bookIds = filtered;
        }   
    }

    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}