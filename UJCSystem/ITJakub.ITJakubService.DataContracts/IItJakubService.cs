﻿using System.Collections.Generic;
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
        string GetBookPageByNameAsync(string bookGuid, string pageName, OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetBookPagesByName(string bookGuid, string startPageName, string endPageName, OutputFormatEnumContract resultFormat);

        [OperationContract]
        string GetBookPageByPosition(string bookGuid, int position, OutputFormatEnumContract resultFormat);

        [OperationContract]
        IList<BookPageContract> GetBookPageList(string bookGuid);

        #region Resource Import
        [OperationContract]       
        void AddResource(UploadResourceContract uploadFileInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);
        #endregion

        [OperationContract]
        List<SearchResultContract> Search(string term);

        [OperationContract]
        BookInfoContract GetBookInfo(string bookGuid);

        [OperationContract]
        BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType);

        [OperationContract]
        Stream GetBookPageImage(BookPageImageContract bookPageImageContract);

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

        
    }
}