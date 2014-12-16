using System;
using System.Collections.Generic;
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

        public string GetBookPageByName(string bookId, string pageName, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormatEnum);
            return m_searchServiceClient.GetBookPageByName(bookId, bookVersion.VersionId, pageName, transformation.Name);
        }

        public string GetBookPagesByName(string bookId, string startPageName, string endPageName, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormatEnum);
            return m_searchServiceClient.GetBookPagesByName(bookId, bookVersion.VersionId, startPageName, endPageName, transformation.Name);
        }

        public string GetBoookPagesByPosition(string bookId, int position, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormatEnum);
            return m_searchServiceClient.GetBookPageByPosition(bookId, bookVersion.VersionId, position, transformation.Name);
        }

        public IList<BookPage> GetBookPagesList(string bookId)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookId);
            return m_searchServiceClient.GetBookPageList(bookId, bookVersion.VersionId);
        }


    }
}