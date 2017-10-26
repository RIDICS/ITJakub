using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.SearchService.DataContracts;
using ITJakub.SearchService.DataContracts.Types;
using ITJakub.Shared.Contracts;
using log4net;
using Vokabular.Core.Storage;
using Vokabular.Shared.DataContracts.Types;
using BookContract = ITJakub.MobileApps.MobileContracts.BookContract;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly AuthorizationManager m_authorizationManager;
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly SearchServiceClient m_searchServiceClient;

        public BookManager(SearchServiceClient searchServiceClient, BookRepository bookRepository,
            BookVersionRepository bookVersionRepository, FileSystemManager fileSystemManager,
            AuthorizationManager authorizationManager)
        {
            m_searchServiceClient = searchServiceClient;
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
            m_fileSystemManager = fileSystemManager;
            m_authorizationManager = authorizationManager;
        }

        public bool HasBookPageByXmlId(string bookGuid, string versionId)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start MainService (BookManager) get version '{0}' of book '{1}'", versionId,
                    bookGuid);

            m_authorizationManager.AuthorizeBook(bookGuid);

            return m_bookVersionRepository.CountBookPageByXmlId(bookGuid, versionId) > 0;
        }

        public string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat,
            BookTypeEnumContract bookTypeContract)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId,
                    bookGuid);

            m_authorizationManager.AuthorizeBook(bookGuid);

            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            BookTypeEnum bookType;
            if (!Enum.TryParse(bookTypeContract.ToString(), true, out bookType))
            {
                throw new ArgumentException(string.Format("Book type : '{0}' unknown", bookTypeContract));
            }

            var bookVersion = m_bookRepository.GetLastVersionForBookWithType(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
            var pageText = m_searchServiceClient.GetBookPageByXmlId(bookGuid, bookVersion.VersionId, pageXmlId,
                transformationName, resultFormat, transformationLevel);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId,
                    bookGuid);

            return pageText;
        }

        public IList<BookPageContract> GetBookPagesList(string bookGuid)
        {
            m_authorizationManager.AuthorizeBook(bookGuid);
            var pages = m_bookVersionRepository.GetLastVersionPageList(bookGuid);
            return Mapper.Map<IList<BookPageContract>>(pages);
        }

        public IList<PageContract> GetBookPagesListMobile(string bookGuid)
        {
            var pages = m_bookVersionRepository.GetLastVersionPageList(bookGuid);
            return Mapper.Map<IList<PageContract>>(pages);
        }

        public IList<BookContentItemContract> GetBookContent(string bookGuid)
        {
            m_authorizationManager.AuthorizeBook(bookGuid);
            var bookContentItems = m_bookVersionRepository.GetRootBookContentItemsWithPagesAndAncestors(bookGuid);
            var contentItemsContracts = Mapper.Map<IList<BookContentItemContract>>(bookContentItems);
            return contentItemsContracts;
        }

        public BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid)
        {
            m_authorizationManager.AuthorizeBook(bookGuid);
            var bookVersion = m_bookRepository.GetLastVersionForBookWithPages(bookGuid);
            return Mapper.Map<BookInfoWithPagesContract>(bookVersion);
        }

        public bool HasBookImage(string bookXmlId, string versionId)
        {
            m_authorizationManager.AuthorizeBook(bookXmlId);

            return m_bookVersionRepository.CountBookImageByXmlId(bookXmlId, versionId) > 0;
        }

        public Stream GetBookPageImage(string bookXmlId, int position)
        {
            m_authorizationManager.AuthorizeBook(bookXmlId);

            var bookPage = m_bookVersionRepository.FindBookPageByXmlIdAndPosition(bookXmlId, position);

            if (bookPage.Image != null)
                return m_fileSystemManager.GetResource(bookXmlId, bookPage.BookVersion.VersionId,
                    bookPage.Image, ResourceType.Image);

            return Stream.Null;
        }

        public Stream GetBookPageImage(string bookXmlId, string pageId)
        {
            var bookPage = m_bookVersionRepository.FindBookPageByXmlId(bookXmlId, pageId);

            if (bookPage.Image != null)
                return m_fileSystemManager.GetResource(bookXmlId, bookPage.BookVersion.VersionId,
                    bookPage.Image, ResourceType.Image);

            return Stream.Null;
        }

        public Stream GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName)
        {
            m_authorizationManager.AuthorizeBook(bookXmlId);
            return m_fileSystemManager.GetResource(bookXmlId, bookVersionXmlId, fileName, ResourceType.Image);
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId,
            OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            m_authorizationManager.AuthorizeBook(bookGuid);

            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            var bookVersion = m_bookRepository.GetLastVersionForBookWithType(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
            var dictionaryEntryText = m_searchServiceClient.GetDictionaryEntryByXmlId(bookGuid, bookVersion.VersionId,
                xmlEntryId, transformationName, resultFormat, transformationLevel);

            return dictionaryEntryText;
        }

        public BookContract GetBookInfoMobile(string bookGuid)
        {
            var bookVersion = m_bookVersionRepository.GetBookVersionWithAuthorsByGuid(bookGuid);
            if (bookVersion == null)
                throw new FaultException("Not found");

            var bookContract = Mapper.Map<BookContract>(bookVersion);
            return bookContract;
        }

        public IList<TermContract> GetTermsOnPage(string bookXmlId, string pageXmlId)
        {
            m_authorizationManager.AuthorizeBook(bookXmlId);
            var terms = m_bookVersionRepository.GetTermsOnPage(bookXmlId, pageXmlId);
            return Mapper.Map<IList<TermContract>>(terms);
        }

        public IList<TermCategoryContract> GetTermCategoriesWithTerms()
        {
            var termCategoriesWithTerms = m_bookRepository.GetTermCategoriesWithTerms();
            return Mapper.Map<IList<TermCategoryContract>>(termCategoriesWithTerms);
        }


        public string GetBookEditionNote(long bookId, OutputFormatEnumContract resultFormat)
        {
            m_authorizationManager.AuthorizeBook(bookId);

            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var book = m_bookRepository.FindBookById(bookId);
            var bookVersion = m_bookRepository.GetLastVersionForBookByBookId(bookId);
            var bookType = m_bookVersionRepository.GetBookTypeByBookVersionId(bookVersion.Id);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType.Type);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;

            var editionNoteText = m_searchServiceClient.GetBookEditionNote(book.Guid, bookVersion.VersionId,
                transformationName, resultFormat, transformationLevel);

            return editionNoteText;
        }

        public long GetBookIdByXmlId(string bookGuid)
        {
            var book = m_bookRepository.FindBookByGuid(bookGuid);
            var bookId = book.Id;

            m_authorizationManager.AuthorizeBook(bookId);

            return bookId;
        }
    }
}