using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubServiceEncrypted
    {
        #region User Operations
        [OperationContract]
        UserContract FindUserById(int userId);
        
        [OperationContract]
        UserContract FindUserByUserName(string userName);

        [OperationContract]
        UserContract CreateUser(UserContract user);
        #endregion

        #region Favorite Items

        [OperationContract]
        List<PageBookmarkContract> GetPageBookmarks(string bookId, string userName);

        [OperationContract]
        void AddBookmark(string bookId, string pageName, string userName);

        [OperationContract]
        void RemoveBookmark(string bookId, string pageName, string userName);

        #endregion

     
    }
}