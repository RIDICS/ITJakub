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

        public AuthorizationManager(UserRepository mUserRepository, PermissionRepository mPermissionRepository)
        {
            m_userRepository = mUserRepository;
            m_permissionRepository = mPermissionRepository;
        }

        private User GetCurrentUser()
        {
            //var username = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            var username = "testUser"; //TODO HACK
            return m_userRepository.FindByUserName(username);
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

        public void FilterBooks(ref IList<BookVersion> books)
        {
            var user = GetCurrentUser();
            var bookIds = books.Select(x => x.Id).ToList();
            var filteredBookIds = m_permissionRepository.GetFilteredBookIdListByUserPermissions(user.Id, bookIds);
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