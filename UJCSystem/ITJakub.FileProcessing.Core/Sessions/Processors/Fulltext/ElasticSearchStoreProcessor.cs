using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts.Contracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.Converters;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ElasticSearchStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;
        private readonly IXmlToTextConverter m_converter;


        public ElasticSearchStoreProcessor(FileProcessingCommunicationProvider communicationProvider,
            IXmlToTextConverter converter)
        {
            m_communicationProvider = communicationProvider;
            m_converter = converter;
        }

        public FulltextStoreTypeContract StoreType => FulltextStoreTypeContract.ElasticSearch;

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            var client = m_communicationProvider.GetFulltextServiceClient();
            return client.CreateTextResource(m_converter.Convert(resourceUploadContract.DataStream), 1);
        }

        public void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void PublishSnapshot(long snapshotId, long projectId, List<string> externalPageIds,
            MetadataResource metadata)
        {
            var snapshotResource = new SnapshotPageIdsResourceContract
            {
                PageIds = externalPageIds,
                SnapshotId = snapshotId,
                ProjectId = projectId,
                MetadataResource = new SnapshotMetadataResourceContract
                {
                    Title = metadata.Title,
                    SubTitle = metadata.SubTitle,
                    AuthorsLabel = metadata.AuthorsLabel,
                    RelicAbbreviation = metadata.RelicAbbreviation,
                    SourceAbbreviation = metadata.SourceAbbreviation,
                    PublishPlace = metadata.PublishPlace,
                    PublishDate = metadata.PublishDate,
                    PublisherText = metadata.PublisherText,
                    PublisherEmail = metadata.PublisherEmail,
                    Copyright = metadata.Copyright,
                    BiblText = metadata.BiblText,
                    OriginDate = metadata.OriginDate,
                    NotBefore = metadata.NotBefore,
                    NotAfter = metadata.NotAfter,
                    ManuscriptIdno = metadata.ManuscriptIdno,
                    ManuscriptSettlement = metadata.ManuscriptSettlement,
                    ManuscriptCountry = metadata.ManuscriptCountry,
                    ManuscriptRepository = metadata.ManuscriptRepository,
                    ManuscriptExtent = metadata.ManuscriptExtent,
                    ManuscriptTitle = metadata.ManuscriptTitle
                }
            };

            var client = m_communicationProvider.GetFulltextServiceClient();
            client.CreateSnapshot(snapshotResource);
        }
    }
}