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
        private readonly UserRepository m_userRepository;

        public AuthorizationManager(UserRepository userRepository, PermissionRepository permissionRepository)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
        }

        public User GetCurrentUser()
        {
            //var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            var username = "testUser"; //TODO HACK
            var user =  m_userRepository.FindByUserName(username);
            if (user == null)
            {
                throw new AuthorizationException(string.Format("Cannot find user with username '{0}'. Probably does not exist.", username));
            }

            return user;
        }

        public void CheckUserCanAddNews()
        {
            var user = GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var newsPermissions = specialPermissions.OfType<NewsPermission>();
            if (!newsPermissions.Any(x => x.CanAddNews))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to add news", user.UserName));
            }

        }

        public void CheckUserCanManageFeedbacks()
        {
            var user = GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var feedbackPermissions = specialPermissions.OfType<FeedbackPermission>();
            if (!feedbackPermissions.Any(x => x.CanManageFeedbacks))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to manage feedbacks", user.UserName));
            }

        } 

        public void CheckUserCanManagePermissions()
        {
            var user = GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var managePermissionsPermissions = specialPermissions.OfType<ManagePermissionsPermission>();
            if (!managePermissionsPermissions.Any(x => x.CanManagePermissions))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to manage permissions", user.UserName));
            }
        } 

        public void CheckUserCanUploadBook()
        {
            var user = GetCurrentUser();
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByUser(user.Id);
            var uploadBookPermissions = specialPermissions.OfType<UploadBookPermission>();
            if (!uploadBookPermissions.Any(x => x.CanUploadBook))
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission to upload books", user.UserName));
            }
        } 

        public void AuthorizeBook(string bookXmlId)
        {
            var user = GetCurrentUser();
            var filtered = m_permissionRepository.GetFilteredBookXmlIdListByUserPermissions(user.Id, new List<string> {bookXmlId});
            if (filtered == null)
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission on book with xmlId '{1}'", user.UserName,
                    bookXmlId));
            }
        }

        public void AuthorizeBook(long bookId)
        {
            var user = GetCurrentUser();
            var filtered = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, new List<long> {bookId});
            if (filtered == null)
            {
                throw new AuthorizationException(string.Format("User with username '{0}' does not have permission on book with id '{1}'", user.UserName,
                    bookId));
            }
        }

        public void FilterBooksByCurrentUser(ref IList<BookVersion> books)
        {
            var user = GetCurrentUser();

            if (books == null || books.Count == 0)
            {
                return;
            }

            var bookIds = books.Select(x => x.Id).ToList();
            var filteredBookIds = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, bookIds);
            var filteredBooks = books.Where(x => filteredBookIds.Contains(x.Id)).ToList();
            books = filteredBooks;
        }

        public void FilterBooksByGroup(int groupId, ref IList<BookVersion> books)
        {
            if (books == null || books.Count == 0)
            {
                return;
            }

            var bookIds = books.Select(x => x.Id).ToList();
            var filteredBookIds = m_permissionRepository.GetFilteredBookIdListByGroupPermissions(groupId, bookIds);
            var filteredBooks = books.Where(x => filteredBookIds.Contains(x.Id)).ToList();
            books = filteredBooks;
        }

        public void AuthorizeCriteria(List<SearchCriteriaContract> searchCriteriaContracts)
        {
            var user = GetCurrentUser();
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
            var user = GetCurrentUser();

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