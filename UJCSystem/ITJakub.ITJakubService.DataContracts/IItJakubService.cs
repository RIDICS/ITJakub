using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        [OperationContract]
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        string GetBookPageByName(string bookGuid, string pageName, OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetBookPagesByName(string bookGuid, string startPageName, string endPageName,
            OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetBookPageByPosition(string bookGuid, int position, OutputFormatEnumContract resultFormat);

        [OperationContract]
        IEnumerable<BookPageContract> GetBookPageList(string bookGuid);

        [OperationContract]
        IEnumerable<BookContentItemContract> GetBookContent(string bookGuid);

        #region Resource Import

        [OperationContract]
        void AddResource(UploadResourceContract uploadFileInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);

        #endregion

        [OperationContract]
        IEnumerable<SearchResultContract> Search(string term);

        [OperationContract]
        IEnumerable<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType);

        [OperationContract]
        IEnumerable<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType);

        [OperationContract]
        BookInfoContract GetBookInfo(string bookGuid);

        [OperationContract]
        BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType);

        [OperationContract]
        Stream GetBookPageImage(BookPageImageContract bookPageImageContract);

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
        IList<string> GetTypeaheadDictionaryHeadwords(string query);

        [OperationContract]
        IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType);

        [OperationContract]
        IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType);

        #endregion

        [OperationContract]
        IList<HeadwordContract> SearchHeadword(string query, IList<string> dictionaryGuidList, int page, int pageSize);
    }
}