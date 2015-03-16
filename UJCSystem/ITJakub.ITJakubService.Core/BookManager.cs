using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private readonly SearchServiceClient m_searchServiceClient;
        private readonly BookRepository m_bookRepository;

        public BookManager(SearchServiceClient searchServiceClient, BookRepository bookRepository)
        {
            m_searchServiceClient = searchServiceClient;
            m_bookRepository = bookRepository;
        }

        public async Task<string> GetBookPageByNameAsync(string bookId, string pageName, string resultFormat)
        {
            OutputFormat outputFormat;
            Enum.TryParse(resultFormat, true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            //var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat);
            //var transformationName = transformation.Name; //TODO make transformation resolving and upload to DB
            var transformationName = "pageToHtml.xsl"; //TODO make transformation resolving and upload to DB
            return await m_searchServiceClient.GetBookPageByNameAsync(bookId, bookVersion.VersionId, pageName, transformationName);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookId, string startPageName, string endPageName, string resultFormat)
        {
            OutputFormat outputFormat;
            Enum.TryParse(resultFormat, true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            //var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat);
            //var transformationName = transformation.Name; //TODO make transformation resolving and upload to DB
            var transformationName = "pageToHtml.xsl"; //TODO make transformation resolving and upload to DB
            return await m_searchServiceClient.GetBookPagesByNameAsync(bookId, bookVersion.VersionId, startPageName, endPageName, transformationName);
        }

        public async Task<string> GetBookPagesByPositionAsync(string bookId, int position, string resultFormat)
        {
            OutputFormat outputFormat;
            Enum.TryParse(resultFormat, true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            //var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat);
            //var transformationName = transformation.Name; //TODO make transformation resolving and upload to DB
            var transformationName = "pageToHtml.xsl"; //TODO make transformation resolving and upload to DB
            return await m_searchServiceClient.GetBookPageByPositionAsync(bookId, bookVersion.VersionId, position, transformationName);
        }

        public async Task<IList<BookPage>> GetBookPagesListAsync(string bookId)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            return await m_searchServiceClient.GetBookPageListAsync(bookId, bookVersion.VersionId);
        }


    }
}