using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        [OperationContract]
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract);

        [OperationContract]
        IEnumerable<BookPageContract> GetBookPageList(string bookGuid);

        [OperationContract]
        IEnumerable<BookContentItemContract> GetBookContent(string bookGuid);

        #region Resource Import     

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);

        #endregion

        [OperationContract]
        IEnumerable<SearchResultContract> Search(string term); // TODO probably remove

        [OperationContract]
        BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid);

        [OperationContract]
        BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType);

        [OperationContract]
        Stream GetBookPageImage(string bookXmlId, int position);

        [OperationContract]
        Stream GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName);

        [OperationContract]
        IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias);

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
        IList<string> GetTypeaheadTitles(string query);

        [OperationContract]
        IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query);

        [OperationContract]
        IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType);

        [OperationContract]
        IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds, IList<long> selectedBookIds);

        #endregion

        [OperationContract]
        int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds);

        [OperationContract]
        HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int count);

        [OperationContract]
        long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query);

        [OperationContract]
        long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId);

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
        string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat);

        [OperationContract]
        PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId,
            string pageXmlId, OutputFormatEnumContract resultFormat);


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
    }
}