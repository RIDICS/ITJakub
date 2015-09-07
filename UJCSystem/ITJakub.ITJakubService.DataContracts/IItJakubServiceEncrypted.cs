using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;

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
        void AddPageBookmark(string bookId, string pageName, string userName);

        [OperationContract]
        void RemovePageBookmark(string bookId, string pageName, string userName);

        [OperationContract]
        IList<HeadwordBookmarkContract> GetHeadwordBookmarks(string userName);

        [OperationContract]
        void AddHeadwordBookmark(string bookXmlId, string entryXmlId, string userName);

        [OperationContract]
        void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId, string userName);

        #endregion

        #region Feedback

        [OperationContract]
        void CreateFeedback(string feedback, string username, FeedbackCategoryEnumContract category);

        [OperationContract]
        void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string username);

        #endregion
    }
}