using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using ITJakub.Core;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private readonly SearchServiceClient m_searchServiceClient;
        private readonly BookRepository m_bookRepository;
        private readonly FileSystemManager m_fileSystemManager;

        public BookManager(SearchServiceClient searchServiceClient, BookRepository bookRepository, FileSystemManager fileSystemManager)
        {
            m_searchServiceClient = searchServiceClient;
            m_bookRepository = bookRepository;
            m_fileSystemManager = fileSystemManager;
        }

        public async Task<string> GetBookPageByNameAsync(string bookGuid, string pageName, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            return await m_searchServiceClient.GetBookPageByNameAsync(bookGuid, bookVersion.VersionId, pageName, transformationName, transformationLevel);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookGuid, string startPageName, string endPageName, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name; 
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            return await m_searchServiceClient.GetBookPagesByNameAsync(bookGuid, bookVersion.VersionId, startPageName, endPageName, transformationName, transformationLevel);
        }

        public async Task<string> GetBookPagesByPositionAsync(string bookGuid, int position, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name; 
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            return await m_searchServiceClient.GetBookPageByPositionAsync(bookGuid, bookVersion.VersionId, position, transformationName, transformationLevel);
        }

        public async Task<IList<BookPageContract>> GetBookPagesListAsync(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            return await m_searchServiceClient.GetBookPageListAsync(bookGuid, bookVersion.VersionId);
        }


        public BookInfoContract GetBookInfo(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            return Mapper.Map<BookInfoContract>(bookVersion);
        }

        public Stream GetBookPageImage(BookPageImageContract imageContract)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(imageContract.BookGuid);
            var bookPage = m_bookRepository.FindBookPageByVersionAndPosition(bookVersion.Id, imageContract.Position);
            var imageFileName = bookPage.Image;
            imageFileName = "junslov.jpg"; //TODO test
            return m_fileSystemManager.GetResource(imageContract.BookGuid, bookVersion.VersionId,
                imageFileName, ResourceType.Image);
        }
    }
}