using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.Shared.Contracts;
using Vokabular.Shared.Converters;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ElasticSearchStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;
        private readonly IXmlToTextConverter m_converter;


        public ElasticSearchStoreProcessor(FileProcessingCommunicationProvider communicationProvider, IXmlToTextConverter converter)
        {
            m_communicationProvider = communicationProvider;
            m_converter = converter;
        }

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            
            

            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                

                return client.CreateTextResource(m_converter.Convert(resourceUploadContract.DataStream), 1);
            }
        }

        public void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void PublishSnapshot(long projectId, List<string> externalPageIds)
        {
            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                StringBuilder builder = new StringBuilder();
                foreach (var pageId in externalPageIds)
                {
                    var textResource = client.GetTextResource(pageId, TextFormatEnumContract.Raw);
                    builder.Append(textResource.Text);
                }
                client.CreateSnapshot(projectId, externalPageIds);
            }
        }
    }
}