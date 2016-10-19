using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
using ITJakub.Shared.Contracts.News;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService: IDisposable
    {
        [OperationContract]
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        bool HasBookPageByXmlId(string bookGuid, string versionId);

        [OperationContract]
        string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract);

        [OperationContract]
        long GetBookIdByXmlId(string bookGuid);

        [OperationContract]
        IEnumerable<BookPageContract> GetBookPageList(string bookGuid);

        [OperationContract]
        IEnumerable<BookContentItemContract> GetBookContent(string bookGuid);

        #region Resource Import     

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);

        #endregion

        [OperationContract]
        BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid);

        [OperationContract]
        BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType);

        [OperationContract]
        bool HasBookImage(string bookXmlId, string versionId);

        [OperationContract]
        Stream GetBookPageImage(string bookXmlId, int position);

        [OperationContract]
        Stream GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName);

        [OperationContract]
        IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds, BookTypeEnumContract bookType);

        [OperationContract]
        HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int count, BookTypeEnumContract bookType);

        [OperationContract]
        long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, BookTypeEnumContract bookType);

        [OperationContract]
        long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId, BookTypeEnumContract bookType);

        [OperationContract]
        HeadwordListContract SearchHeadwordByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget);

        [OperationContract]
        int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget);

        [OperationContract]
        CorpusSearchResultContractList GetCorpusSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetCorpusSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int SearchCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookType);

        [OperationContract]
        string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid, string xmlEntryId,
            OutputFormatEnumContract resultFormat, BookTypeEnumContract bookType);

        [OperationContract]
        PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId,
            string pageXmlId, OutputFormatEnumContract resultFormat);

        [OperationContract]
        IList<TermContract> GetTermsOnPage(string bookXmlId, string pageXmlId);

        #region CardFile methods

        [OperationContract]
        IEnumerable<CardFileContract> GetCardFiles();

        [OperationContract]
        IEnumerable<BucketShortContract> GetBuckets(string cardFileId);

        [OperationContract]
        IEnumerable<BucketShortContract> GetBucketsWithHeadword(string cardFileId, string headword);

        [OperationContract]
        IEnumerable<CardContract> GetCards(string cardFileId, string bucketId);

        [OperationContract]
        IEnumerable<CardShortContract> GetCardsShort(string cardFileId, string bucketId);

        [OperationContract]
        CardContract GetCard(string cardFileId, string bucketId, string cardId);

        [OperationContract]
        Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId, ImageSizeEnum imageSize);

        #endregion

        #region Typeahead methods

        [OperationContract]
        IList<string> GetTypeaheadAuthors(string query);

        [OperationContract]
        IList<UserContract> GetTypeaheadUsers(string query);

        [OperationContract]
        IList<GroupContract> GetTypeaheadGroups(string query);

        [OperationContract]
        IList<string> GetTypeaheadTitles(string query);

        [OperationContract]
        IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, BookTypeEnumContract? bookType);

        [OperationContract]
        IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType);

        [OperationContract]
        IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds, IList<long> selectedBookIds);

        [OperationContract]
        IList<string> GetTypeaheadTermsByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds, IList<long> selectedBookIds);

        #endregion

        #region Feedback

        [OperationContract]
        void CreateAnonymousFeedback(string feedback, string name, string email, FeedbackCategoryEnumContract feedbackCategory);

        [OperationContract]
        void CreateAnonymousFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string name, string email);

        [OperationContract]
        List<FeedbackContract> GetFeedbacks(FeedbackCriteriaContract feedbackSearchCriteria);

        [OperationContract]
        int GetFeedbacksCount(FeedbackCriteriaContract feedbackSearchCriteria);

        [OperationContract]
        void DeleteFeedback(long feedbackId);

        #endregion

        #region AudioBooks

        [OperationContract]
        AudioBookSearchResultContractList GetAudioBooksSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetAudioBooksSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias);

        #endregion

        #region Permissions

        [OperationContract]
        IList<GroupContract> GetGroupsByUser(int userId);

        [OperationContract]
        IList<UserContract> GetUsersByGroup(int groupId);

        [OperationContract]
        void AddUserToGroup(int userId, int groupId);

        [OperationContract]
        void RemoveUserFromGroup(int userId, int groupId);

        [OperationContract]
        GroupContract CreateGroup(string name, string description);

        [OperationContract]
        UserDetailContract GetUserDetail(int userId);

        [OperationContract]
        GroupDetailContract GetGroupDetail(int groupId);

        [OperationContract]
        IList<CategoryContract> GetRootCategories();

        [OperationContract]
        CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId);

        [OperationContract]
        CategoryContentContract GetAllCategoryContent(int categoryId);

        [OperationContract]
        void DeleteGroup(int groupId);

        [OperationContract]
        void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds);

        [OperationContract]
        void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds);

        [OperationContract]
        IList<SpecialPermissionContract> GetSpecialPermissions();

        [OperationContract]
        IList<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId);

        [OperationContract]
        IList<SpecialPermissionContract> GetSpecialPermissionsForUser();

        [OperationContract]
        IList<SpecialPermissionContract> GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract permissionType);

        [OperationContract]
        void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionIds);

        [OperationContract]
        void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionIds);

        #endregion

        #region News

        [OperationContract]
        List<NewsSyndicationItemContract> GetWebNewsSyndicationItems(int start, int count);

        [OperationContract]
        int GetWebNewsSyndicationItemCount();

        #endregion

        #region Favorite Items

        [OperationContract]
        IList<FavoriteLabelContract> GetFavoriteLabels(int latestLabelCount);

        [OperationContract]
        long CreateFavoriteLabel(string name, string color);

        [OperationContract]
        void UpdateFavoriteLabel(long labelId, string name, string color);

        [OperationContract]
        void DeleteFavoriteLabel(long labelId);

        [OperationContract]
        IList<FavoriteBaseInfoContract> GetFavoriteItems(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle, FavoriteSortContract sort, int start, int count);
        
        [OperationContract]
        int GetFavoriteItemsCount(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle);

        [OperationContract]
        IList<FavoriteQueryContract> GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle, int start, int count);

        [OperationContract]
        int GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle);

        [OperationContract]
        List<PageBookmarkContract> GetPageBookmarks(string bookId);
        
        [OperationContract]
        IList<HeadwordBookmarkContract> GetHeadwordBookmarks();

        [OperationContract]
        FavoriteFullInfoContract GetFavoriteItem(long id);

        [OperationContract]
        void AddHeadwordBookmark(string bookXmlId, string entryXmlId);

        [OperationContract]
        void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId);

        [OperationContract]
        IList<FavoriteBookInfoContract> GetFavoriteLabeledBooks(IList<long> bookIds);

        [OperationContract]
        IList<FavoriteCategoryContract> GetFavoriteLabeledCategories(IList<int> categoryIds);

        [OperationContract]
        IList<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType);

        [OperationContract]
        void UpdateFavoriteItem(long id, string title);

        [OperationContract]
        void DeleteFavoriteItem(long id);

        [OperationContract]
        IList<long> CreateFavoriteBook(long bookId, string title, IList<long> labelIds);

        [OperationContract]
        IList<long> CreateFavoriteCategory(int categoryId, string title, IList<long> labelIds);

        [OperationContract]
        IList<long> CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, IList<long> labelIds);

        [OperationContract]
        long CreatePageBookmark(string bookXmlId, string pageXmlId, string title, long? labelId);

        #endregion

        #region Feedback

        [OperationContract]
        void CreateFeedback(string feedback, string username, FeedbackCategoryEnumContract category);

        [OperationContract]
        void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string username);

        #endregion

        #region News

        [OperationContract]
        void CreateNewsSyndicationItem(string title, string content, string url, NewsTypeContract itemType, string username);

        #endregion

        [OperationContract]
        IList<TermCategoryContract> GetTermCategoriesWithTerms();

        [OperationContract]
        string GetBookEditionNote(long bookId, OutputFormatEnumContract outputFormat);
    }
}