using System;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateMetadataSubtask
    {
        private readonly MetadataRepository m_metadataRepository;

        public UpdateMetadataSubtask(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public void UpdateMetadata(long projectId, int userId, BookData bookData)
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(projectId);
            var firstManuscript = bookData.ManuscriptDescriptions?.FirstOrDefault();

            var authorsString = bookData.Authors != null
                ? string.Join(", ", bookData.Authors.Select(x => x.Name))
                : null;
            var metadata = new MetadataResource
            {
                AuthorsLabel = authorsString,
                BiblText = bookData.BiblText,
                Comment = string.Empty,
                Copyright = bookData.Copyright,
                CreatedByUser = m_metadataRepository.Load<User>(userId),
                CreateTime = now,
                PublisherText = bookData.Publisher?.Text,
                PublisherEmail = bookData.Publisher?.Email,
                PublishDate = bookData.PublishDate,
                PublishPlace = bookData.PublishPlace,
                SourceAbbreviation = bookData.SourceAbbreviation,
                RelicAbbreviation = bookData.RelicAbbreviation,
                Title = bookData.Title,
                SubTitle = bookData.SubTitle
            };

            if (lastMetadata == null)
            {
                var resource = new Resource
                {
                    Project = m_metadataRepository.Load<Project>(projectId),
                    ContentType = ContentTypeEnum.None,
                    Name = "Metadata",
                    ResourceType = ResourceTypeEnum.ProjectMetadata
                };

                metadata.Resource = resource;
                metadata.VersionNumber = 1;
            }
            else
            {
                metadata.Resource = lastMetadata.Resource;
                metadata.VersionNumber = lastMetadata.VersionNumber + 1;
            }

            metadata.Resource.LatestVersion = metadata;

            if (firstManuscript != null)
            {
                metadata.ManuscriptSettlement = firstManuscript.Settlement;
                metadata.ManuscriptCountry = firstManuscript.Country;
                metadata.ManuscriptExtent = firstManuscript.Extent;
                metadata.ManuscriptIdno = firstManuscript.Idno;
                metadata.ManuscriptRepository = firstManuscript.Repository;
                metadata.ManuscriptTitle = firstManuscript.Title;
                metadata.NotBefore = firstManuscript.NotBefore;
                metadata.NotAfter = firstManuscript.NotAfter;
                metadata.OriginDate = firstManuscript.OriginDate;
            }

            m_metadataRepository.Create(metadata);
        }
    }
}