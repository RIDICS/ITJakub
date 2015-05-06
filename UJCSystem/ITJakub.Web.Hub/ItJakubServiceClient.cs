using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceClient : ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            try
            {
                return Channel.GetAllAuthors();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors timeouted with: {0}", ex);
                throw;
            }
        }

        public BookInfoContract GetBookInfo(string bookId)
        {
            try
            {
                return Channel.GetBookInfo(bookId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfo failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfo timeouted with: {0}", ex);
                throw;
            }
        }

        public async Task<string> GetBookPageByNameAsync(string documentId, string pageName, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return await Channel.GetBookPageByNameAsync(documentId, pageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByNameAsync failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByNameAsync timeouted with: {0}", ex);
                throw;
            }
        }

        public async Task<string> GetBookPagesByNameAsync(string documentId, string startPageName, string endPageName, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return await Channel.GetBookPagesByNameAsync(documentId, startPageName, endPageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByNameAsync failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByNameAsync timeouted with: {0}", ex);
                throw;
            }
        }

        public async Task<string> GetBookPageByPositionAsync(string documentId, int position, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return await Channel.GetBookPageByPositionAsync(documentId, position, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPositionAsync failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPositionAsync timeouted with: {0}", ex);
                throw;
            }
        }
            
        public async Task<IList<BookPageContract>> GetBookPageListAsync(string documentId)
        {
            try
            {
                return await Channel.GetBookPageListAsync(documentId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageListAsync failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageListAsync timeouted with: {0}", ex);
                throw;
            }
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            try
            {
                Channel.AddResource(resourceInfoSkeleton);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource timeouted with: {0}", ex);
                throw;
            }
        }

        public bool ProcessSession(string sessionId, string uploadMessage)
        {
            try
            {
                return Channel.ProcessSession(sessionId, uploadMessage);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession timeouted with: {0}", ex);
                throw;
            }
        }

        public List<SearchResultContract> Search(string term)
        {
            try
            {
                return Channel.Search(term);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search timeouted with: {0}", ex);
                throw;
            }
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.GetBooksWithCategoriesByBookType(bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public Stream GetBookPageImage(BookPageImageContract bookPageImageContract)
        {
            try
            {
                return Channel.GetBookPageImage(bookPageImageContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<CardFileContract> GetCardFiles()
        {
            try
            {
                return Channel.GetCardFiles();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles timeouted with: {0}", ex);
                throw;
            }
        }
        
        public IEnumerable<BucketContract> GetBuckets(string cardFileId)
        {
            try
            {
                return Channel.GetBuckets(cardFileId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets timeouted with: {0}", ex);
                throw;
            }
        }        
        public IEnumerable<BucketContract> GetBucketsWithHeadword(string cardFileId, string headword)
        {
            try
            {
                return Channel.GetBucketsWithHeadword(cardFileId, headword);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<CardContract> GetCards(string cardFileId, string bucketId)
        {
            try
            {
                return Channel.GetCards(cardFileId, bucketId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards timeouted with: {0}", ex);
                throw;
            }
        }
        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            try
            {
                return Channel.GetCard(cardFileId, bucketId,cardId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard timeouted with: {0}", ex);
                throw;
            }
        }

        public Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            try
            {
                return Channel.GetImage(cardFileId, bucketId, cardId, imageId, imageSize);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage timeouted with: {0}", ex);
                throw;
            }
        }
    }
}