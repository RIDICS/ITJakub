using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using ITJakub.Core;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;
using BookContract = ITJakub.MobileApps.MobileContracts.BookContract;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BookRepository m_bookRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly SearchServiceClient m_searchServiceClient;

        public BookManager(SearchServiceClient searchServiceClient, BookRepository bookRepository, BookVersionRepository bookVersionRepository, FileSystemManager fileSystemManager)
        {
            m_searchServiceClient = searchServiceClient;
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
            m_fileSystemManager = fileSystemManager;
        }

        public string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId,
                    bookGuid);

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
                m_log.DebugFormat("End MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId, bookGuid);

            return pageText;
        }

        public IList<BookPageContract> GetBookPagesList(string bookGuid)
        {
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
            var bookContentItems = m_bookVersionRepository.GetRootBookContentItemsWithPagesAndAncestors(bookGuid);
            var contentItemsContracts = Mapper.Map<IList<BookContentItemContract>>(bookContentItems);
            return contentItemsContracts;
        }

        public BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBookWithPages(bookGuid);
            return Mapper.Map<BookInfoWithPagesContract>(bookVersion);
        }

        public Stream GetBookPageImage(string bookXmlId, int position)
        {
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
            return m_fileSystemManager.GetResource(bookXmlId, bookVersionXmlId, fileName, ResourceType.Image);
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
            }

            var bookType = Mapper.Map<BookTypeEnum>(bookTypeContract);
            var bookVersion = m_bookRepository.GetLastVersionForBookWithType(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            var dictionaryEntryText = m_searchServiceClient.GetDictionaryEntryByXmlId(bookGuid, bookVersion.VersionId, xmlEntryId, transformationName, resultFormat, transformationLevel);

            return dictionaryEntryText;
        }

        public BookContract GetBookInfoMobile(string bookGuid)
        {
            var bookVersion = m_bookVersionRepository.GetBookVersionWithAuthorsByGuid(bookGuid);
            var bookContract = Mapper.Map<BookContract>(bookVersion);
            return bookContract;
        }
    }
}