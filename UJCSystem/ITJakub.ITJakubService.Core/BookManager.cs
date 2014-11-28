using System;
using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.Core.SearchService;
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

        public void CreateBook(string bookGuid, string name, string author)
        {
            m_bookRepository.CreateBook(bookGuid, name, author);
        }

        public void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds)
        {
            m_bookRepository.AssignAuthorsToBook(bookGuid, bookVersionGuid, authorIds);
        }

        public string GetBookPageByName(string documentId, string pageName, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var transformationName = m_bookRepository.FindTransformationName(documentId, resultFormat);
            return m_searchServiceClient.GetBookPageByName(documentId, pageName, transformationName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var transformationName = m_bookRepository.FindTransformationName(documentId, resultFormat);
            return m_searchServiceClient.GetBookPagesByName(documentId, startPageName, endPageName, transformationName);
        }

        public string GetBoookPagesByPosition(string documentId, int position, string resultFormat)
        {
            OutputFormatEnum outputFormatEnum;
            Enum.TryParse(resultFormat, true, out outputFormatEnum);
            var transformationName = m_bookRepository.FindTransformationName(documentId, resultFormat);
            return m_searchServiceClient.GetBookPageByPosition(documentId, position, transformationName);
        }

        public IList<BookPage> GetBookPagesList(string documentId)
        {
            return m_searchServiceClient.GetBookPageList(documentId);
        }


    }
}