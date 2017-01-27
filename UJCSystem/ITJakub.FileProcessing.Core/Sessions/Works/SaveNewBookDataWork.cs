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
        private readonly long? m_nullableProjectId;
        private readonly string m_message;
        private readonly int m_userId;
        private long m_projectId;
        
        public SaveNewBookDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_resourceDirector = resourceDirector;
            m_nullableProjectId = resourceDirector.GetSessionInfoValue<long?>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_message = resourceDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            m_userId = resourceDirector.GetSessionInfoValue<int>(SessionInfo.UserId);
        }

        protected override void ExecuteWorkImplementation()
        {
            UpdateProject();
            
            //TODO update: 1) metadata, authors, editors, category, kind, genre
            //TODO 2) Page list & chapters 3) Headwords 4) Tracks 5) keywords 6) terms 7) transformations

            UpdateAuthors();
            UpdateMetadata();

            UpdateHistoryLog();

            throw new NotImplementedException();
        }

        private void UpdateProject()
        {
            if (m_nullableProjectId != null)
            {
                m_projectId = m_nullableProjectId.Value;

                var project = m_projectRepository.FindById<Project>(m_projectId);
                if (project.ExternalId != m_bookData.BookXmlId)
                {
                    project.ExternalId = m_bookData.BookXmlId;
                    m_projectRepository.Update(project);
                }
            }
            else
            {
                var dbProject = m_projectRepository.GetProjectByExternalId(m_bookData.BookXmlId);
                if (dbProject != null)
                {
                    m_projectId = dbProject.Id;
                }
                else
                {
                    var newProject = new Project
                    {
                        Name = m_bookData.Title,
                        CreateTime = DateTime.UtcNow,
                        CreatedByUser = m_projectRepository.Load<User>(m_userId),
                        ExternalId = m_bookData.BookXmlId,
                    };
                    m_projectId = (long)m_projectRepository.Create(newProject);
                }
            }
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
            newDbAuthor = m_metadataRepository.Load<OriginalAuthor>(newDbAuthor.Id);

            return newDbAuthor;
        }

        private void UpdateMetadata()
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(m_projectId, true);
            var firstManuscript = m_bookData.ManuscriptDescriptions.FirstOrDefault();

            var publisher = GetOrCreatePublisher(m_bookData.Publisher.Text, m_bookData.Publisher.Email);
            var metadata = new MetadataResource
            {
                Resource = lastMetadata.Resource,
                VersionNumber = lastMetadata.VersionNumber + 1,
                BiblText = m_bookData.BiblText,
                Comment = m_message,
                Copyright = m_bookData.Copyright,
                CreatedByUser = m_metadataRepository.Load<User>(m_userId),
                CreateTime = now,
                Publisher = publisher,
                PublishDate = m_bookData.PublishDate,
                PublishPlace = m_bookData.PublishPlace,
                SourceAbbreviation = m_bookData.SourceAbbreviation,
                RelicAbbreviation = m_bookData.RelicAbbreviation,
                Title = m_bookData.Title,
                SubTitle = m_bookData.SubTitle
            };
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

            m_projectRepository.Create(metadata);
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

        private void UpdateHistoryLog()
        {
            var newLog = new FullProjectImportLog
            {
                Project = m_projectRepository.Load<Project>(m_projectId),
                AdditionalDescription = m_message,
                CreateTime = DateTime.UtcNow,
                ExternalId = m_bookData.VersionXmlId,
                Text = "New book full import",
                User = m_projectRepository.Load<User>(m_userId)
            };
            m_projectRepository.Create(newLog);
        }
    }
}
