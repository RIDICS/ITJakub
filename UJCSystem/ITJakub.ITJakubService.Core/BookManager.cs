﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using ITJakub.Core;
using ITJakub.Core.SearchService;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class BookManager
    {
        private readonly SearchServiceClient m_searchServiceClient;
        private readonly BookRepository m_bookRepository;
        private readonly FileSystemManager m_fileSystemManager;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BookManager(SearchServiceClient searchServiceClient, BookRepository bookRepository, FileSystemManager fileSystemManager)
        {
            m_searchServiceClient = searchServiceClient;
            m_bookRepository = bookRepository;
            m_fileSystemManager = fileSystemManager;
        }

        public string GetBookPageByName(string bookGuid, string pageName, OutputFormatEnumContract resultFormat)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Start MainService (BookManager) get page name '{0}' of book '{1}'", pageName, bookGuid);

            var searchServiceClient = new SearchServiceClient();
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name;
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            var pageText = searchServiceClient.GetBookPageByName(bookGuid, bookVersion.VersionId, pageName, transformationName, transformationLevel);

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("End MainService (BookManager) get page name '{0}' of book '{1}'", pageName, bookGuid);

            return pageText;
        }

        public string GetBookPagesByName(string bookGuid, string startPageName, string endPageName, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name; 
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            return m_searchServiceClient.GetBookPagesByName(bookGuid, bookVersion.VersionId, startPageName, endPageName, transformationName, transformationLevel);
        }

        public string GetBookPagesByPosition(string bookGuid, int position, OutputFormatEnumContract resultFormat)
        {
            OutputFormat outputFormat;
            var successfullyConverted = Enum.TryParse(resultFormat.ToString(), true, out outputFormat);
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            var transformation = m_bookRepository.FindTransformation(bookVersion, outputFormat, bookVersion.DefaultBookType.Type); //TODO add bookType as method parameter
            var transformationName = transformation.Name; 
            var transformationLevel = (ResourceLevelEnumContract)transformation.ResourceLevel;
            return m_searchServiceClient.GetBookPageByPosition(bookGuid, bookVersion.VersionId, position, transformationName, transformationLevel);
        }

        public IList<BookPageContract> GetBookPagesList(string bookGuid)
        {
            var bookVersion = m_bookRepository.GetLastVersionForBook(bookGuid);
            return m_searchServiceClient.GetBookPageList(bookGuid, bookVersion.VersionId);
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