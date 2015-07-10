using System;
using System.Collections.Generic;
using System.IO;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;
        private readonly IExistResourceManager m_existResourceManager;
        private readonly FulltextSearchCriteriaDirector m_searchCriteriaDirector;

        public ExistManager(ExistClient existClient, IExistResourceManager existResourceManager, FulltextSearchCriteriaDirector searchCriteriaDirector)
        {
            m_client = existClient;
            m_existResourceManager = existResourceManager;
            m_searchCriteriaDirector = searchCriteriaDirector;
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            m_client.UploadBookFile(bookId, fileName, dataStream);
        }

        public void UploadVersionFile(string bookId, string versionId, string fileName, Stream filStream)
        {
            m_client.UploadVersionFile(bookId, versionId, fileName, filStream);
        }

        public void UploadSharedFile(string fileName, Stream filStream)
        {
            m_client.UploadSharedFile(fileName, filStream);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByName(bookId, versionId, pageName,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPagesByName(bookId, versionId, start, end,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByXmlId(bookId, versionId, pageXmlId,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
               transformationLevel, bookId, versionId);
            return m_client.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId,
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }

        public void ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            ResultRestrictionCriteriaContract resultRestrictionCriteriaContract = null;
            var filteredCriterias = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    filteredCriterias.Add(searchCriteriaContract);
                }
                else if (searchCriteriaContract.Key == CriteriaKey.ResultRestriction)
                {
                    resultRestrictionCriteriaContract = (ResultRestrictionCriteriaContract) searchCriteriaContract;
                }
            }

            if (resultRestrictionCriteriaContract == null)
                return;

            var searchCriteria = new ResultSearchCriteriaContract
            {
                ResultBooks = resultRestrictionCriteriaContract.ResultBooks,
                SearchCriterias = filteredCriterias
            };

            var stringResult = m_client.ListSearchEditionsResults(searchCriteria.ToXml());
        }
    }
}