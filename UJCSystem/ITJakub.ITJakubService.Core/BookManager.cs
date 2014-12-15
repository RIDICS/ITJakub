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