using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.Shared.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ElasticSearchStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;

        public ElasticSearchStoreProcessor(FileProcessingCommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                string text;
                using (var reader = new StreamReader(resourceUploadContract.DataStream))
                {
                    text = reader.ReadToEnd();
                }

                return client.CreateTextResource(text, 1);
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
                client.CreateSnapshot(projectId, builder.ToString());
            }
        }
    }
}