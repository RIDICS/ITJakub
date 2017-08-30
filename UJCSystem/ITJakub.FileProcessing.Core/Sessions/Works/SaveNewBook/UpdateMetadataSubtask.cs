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

        public void UpdateMetadata(long projectId, int userId, string comment, BookData bookData)
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(projectId, true);
            var firstManuscript = bookData.ManuscriptDescriptions?.FirstOrDefault();

            var publisher = GetOrCreatePublisher(bookData.Publisher.Text, bookData.Publisher.Email);
            var metadata = new MetadataResource
            {
                BiblText = bookData.BiblText,
                Comment = comment,
                Copyright = bookData.Copyright,
                CreatedByUser = m_metadataRepository.Load<User>(userId),
                CreateTime = now,
                Publisher = publisher,
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

        private Publisher GetOrCreatePublisher(string publisherText, string email)
        {
            var publisher = m_metadataRepository.GetPublisher(publisherText, email);
            if (publisher == null)
            {
                publisher = new Publisher
                {
                    Text = publisherText,
                    Email = email
                };
                m_metadataRepository.Create(publisher);
                publisher = m_metadataRepository.Load<Publisher>(publisher.Id);
            }
            return publisher;
        }
    }
}