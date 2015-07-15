using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using ITJakub.Core;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

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
                m_log.DebugFormat("Start MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId, bookGuid);

            var searchServiceClient = new SearchServiceClient();
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

            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookType);
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract) transformation.ResourceLevel;
						var pageText = searchServiceClient.GetBookPageByXmlId(bookGuid, bookVersion.VersionId, pageXmlId, transformationName, resultFormat, transformationLevel);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End MainService (BookManager) get page xmlId '{0}' of book '{1}'", pageXmlId, bookGuid);

            return pageText;
        }

        public IList<BookPageContract> GetBookPagesList(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var pages = m_bookVersionRepository.GetPageList(bookVersion);
            return Mapper.Map<IList<BookPageContract>>(pages);
        }

        public IList<PageContract> GetBookPagesListMobile(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var pages = m_bookVersionRepository.GetPageList(bookVersion);
            return Mapper.Map<IList<PageContract>>(pages);
        }

        public IList<BookContentItemContract> GetBookContent(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var bookContentItems = m_bookVersionRepository.GetRootBookContentItemsWithPagesAndAncestors(bookVersion);
            var contentItemsContracts = Mapper.Map<IList<BookContentItemContract>>(bookContentItems);
            return contentItemsContracts;
        }

        public BookInfoContract GetBookInfo(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBookWithPages(bookGuid);
            return Mapper.Map<BookInfoContract>(bookVersion);
        }

        public Stream GetBookPageImage(BookPageImageContract imageContract)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(imageContract.BookXmlId);
            var bookPage = m_bookVersionRepository.FindBookPageByVersionAndPosition(bookVersion, imageContract.Position);

            if (bookPage.Image != null)
                return m_fileSystemManager.GetResource(imageContract.BookXmlId, bookVersion.VersionId,
                    bookPage.Image, ResourceType.Image);

            return Stream.Null;
        }

        public Stream GetBookPageImage(string bookGuid, string pageId)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var bookPage = m_bookVersionRepository.FindBookPageByVersionAndXmlId(bookVersion.Id, pageId);
            if (bookPage.Image != null)
                return m_fileSystemManager.GetResource(bookGuid, bookVersion.VersionId,
                    bookPage.Image, ResourceType.Image);

            return Stream.Null;
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat)
        {
            var searchServiceClient = new SearchServiceClient();
            OutputFormat outputFormat;
            if (!Enum.TryParse(resultFormat.ToString(), true, out outputFormat))
            {
                throw new ArgumentException(string.Format("Result format : '{0}' unknown", resultFormat));
    }

            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            var dictionaryEntryText = searchServiceClient.GetDictionaryEntryByXmlId(bookGuid, bookVersion.VersionId, xmlEntryId, transformationName, resultFormat, transformationLevel);

            return dictionaryEntryText;
        }
    }
}