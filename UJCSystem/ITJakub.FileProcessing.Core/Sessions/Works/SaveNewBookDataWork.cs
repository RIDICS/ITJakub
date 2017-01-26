using System;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class SaveNewBookDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ResourceSessionDirector m_resourceDirector;
        private readonly BookData m_bookData;
        private readonly long m_projectId;
        private readonly string m_message;

        public SaveNewBookDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_resourceDirector = resourceDirector;
            m_projectId = resourceDirector.GetSessionInfoValue<long>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_message = resourceDirector.GetSessionInfoValue<string>(SessionInfo.Message);
        }

        protected override void ExecuteWorkImplementation()
        {
            //TODO update: 1) metadata, authors, editors, category, kind, genre
            //TODO 2) Page list & chapters 3) Headwords 4) Tracks 5) keywords 6) terms 7) transformations

            
        }

        private void UpdateAuthors()
        {
            var dbProjectAuthors = m_metadataRepository.GetProjectOriginalAuthorList(m_projectId, true);
            var dbAuthors = dbProjectAuthors.Select(x => x.OriginalAuthor).ToList();
            var newAuthors = m_bookData.Authors.Select(x => AuthorHelper.ConvertToEntity(x.Name)).ToList();

            var comparer = new AuthorNameEqualityComparer();
            var authorsToAdd = newAuthors.Except(dbAuthors, comparer).ToList();
            var authorsToRemove = dbAuthors.Except(newAuthors, comparer).ToList();

            if (authorsToAdd.Count == 0 && authorsToRemove.Count == 0)
            {
                return;
            }


            foreach (var author in authorsToRemove)
            {
                var projectAuthor = dbProjectAuthors.Single(x => x.OriginalAuthor.Id == author.Id);
                m_metadataRepository.Delete(projectAuthor);
            }


            for (int i = 0; i < newAuthors.Count; i++)
            {
                var newAuthor = newAuthors[i];
                if (authorsToAdd.Contains(newAuthor))
                {
                    var dbAuthor = GetOrCreateAuthor(newAuthor.FirstName, newAuthor.LastName);
                    var newProjectAuthor = new ProjectOriginalAuthor
                    {
                        OriginalAuthor = dbAuthor,
                        Project = m_projectRepository.Load<Project>(m_projectId),
                        Sequence = i + 1
                    };
                    m_metadataRepository.Create(newProjectAuthor);
                }
                else
                {
                    var projectAuthor = dbProjectAuthors.Single(x => x.OriginalAuthor.Id == newAuthor.Id);
                    projectAuthor.Sequence = i + 1;
                    m_metadataRepository.Update(projectAuthor);
                }
            }
        }

        private OriginalAuthor GetOrCreateAuthor(string firstName, string lastName)
        {
            var dbAuthor = m_metadataRepository.GetAuthorByName(firstName, lastName);
            if (dbAuthor != null)
            {
                return dbAuthor;
            }

            var newDbAuthor = new OriginalAuthor
            {
                FirstName = firstName,
                LastName = lastName
            };
            m_metadataRepository.Create(newDbAuthor);

            return newDbAuthor;
        }

        private void UpdateMetadata()
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(m_projectId, true);
            var firstManuscript = m_bookData.ManuscriptDescriptions.FirstOrDefault();

            var metadata = new MetadataResource
            {
                Resource = lastMetadata.Resource,
                VersionNumber = lastMetadata.VersionNumber + 1,
                BiblText = m_bookData.BiblText,
                Comment = m_message,
                Copyright = m_bookData.Copyright,
                CreatedByUser = null, //TODO get user
                CreateTime = now,
                Id = 0, //TODO del
                Publisher = null, //TODO
                ManuscriptSettlement = "",
                ManuscriptCountry = "",
                ManuscriptExtent = "",
                ManuscriptIdno = "",
                ManuscriptRepository = "",
                ManuscriptTitle = "",
                NotBefore = null,
                NotAfter = null,
                PublishDate = m_bookData.PublishDate,
                PublishPlace = m_bookData.PublishPlace,
                SourceAbbreviation = m_bookData.SourceAbbreviation,
                RelicAbbreviation = m_bookData.RelicAbbreviation,
                Title = m_bookData.Title,
                SubTitle = m_bookData.SubTitle,
                OriginDate = null
            };
            metadata.Resource.LatestVersion = metadata;

            // TODO save to DB
        }
    }
}
