using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectImport.Works.Helpers;
using Vokabular.ProjectParsing.Model.Entities;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Works
{
    public class SaveImportedDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly PersonRepository m_personRepository;
        private readonly ProjectImportMetadata m_projectImportMetadata;
        private readonly int m_userId;
        private readonly long m_projectId;


        public SaveImportedDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            CatalogValueRepository catalogValueRepository, PersonRepository personRepository, ProjectImportMetadata projectImportMetadata,
            int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_projectImportMetadata = projectImportMetadata;
            m_userId = userId;
            m_projectId = projectImportMetadata.ProjectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_projectId, true, false, false, true, true, true, false);

            UpdateLiteraryGenres(project);
            UpdateLiteraryOriginals(project);
            UpdateKeywords(project);

            m_catalogValueRepository.Update(project);

            UpdateAuthors(project.Authors);
            UpdateMetadata(project);
            UpdateHistoryLog(project);
        }

        private void UpdateMetadata(Project project)
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(m_projectId);
            var firstManuscript = m_projectImportMetadata.Project.ProjectMetadata.ManuscriptDescriptionData;

            var authorsString = m_projectImportMetadata.Project.Authors != null
                ? string.Join(", ", m_projectImportMetadata.Project.Authors.Select(x => $"{x.LastName} {x.FirstName}"))
                : null;

            var projectMetadata = m_projectImportMetadata.Project.ProjectMetadata;
            var metadata = new MetadataResource
            {
                AuthorsLabel = authorsString,
                BiblText = projectMetadata.BiblText,
                Copyright = projectMetadata.Copyright,
                CreatedByUser = m_metadataRepository.Load<User>(m_userId),
                CreateTime = now,
                PublisherText = projectMetadata.PublisherText,
                PublisherEmail = projectMetadata.PublisherEmail,
                PublishDate = projectMetadata.PublishDate,
                PublishPlace = projectMetadata.PublishPlace,
                SourceAbbreviation = projectMetadata.SourceAbbreviation,
                RelicAbbreviation = projectMetadata.RelicAbbreviation,
                Title = projectMetadata.Title,
                SubTitle = projectMetadata.SubTitle
            };

            if (lastMetadata == null)
            {
                var resource = new Resource
                {
                    Project = project,
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

        private void UpdateHistoryLog(Project project)
        {
            var newLog = new FullProjectImportLog
            {
                Project = project,
                CreateTime = DateTime.UtcNow,
                ExternalId = m_projectImportMetadata.ExternalId,
                Text = "New book metadata import from external repository",
                User = m_projectRepository.Load<User>(m_userId)
            };
            m_projectRepository.Create(newLog);
        }

        private void UpdateKeywords(Project project)
        {
            project.Keywords.Clear();

            foreach (var newKeywordName in m_projectImportMetadata.Project.Keywords)
            {
                var dbKeyword = m_catalogValueRepository.GetKeywordByName(newKeywordName);

                // Create new Keyword
                if (dbKeyword == null)
                {
                    dbKeyword = new Keyword
                    {
                        Text = newKeywordName
                    };
                    m_catalogValueRepository.Create(dbKeyword);
                }
                // Assign existing Keyword to project
                if (project.Keywords.All(x => x.Id != dbKeyword.Id))
                {
                    project.Keywords.Add(dbKeyword);
                }
            }
        }

        private void UpdateLiteraryOriginals(Project project)
        {
            project.LiteraryOriginals.Clear();
            var dbOriginalList = m_catalogValueRepository.GetLiteraryOriginalList();

            foreach (var newOriginalName in m_projectImportMetadata.Project.LiteraryOriginals)
            {
                var dbOriginal = dbOriginalList.FirstOrDefault(x => x.Name == newOriginalName);

                // Create new Literary Original
                if (dbOriginal == null)
                {
                    dbOriginal = new LiteraryOriginal
                    {
                        Name = newOriginalName
                    };
                    m_catalogValueRepository.Create(dbOriginal);
                    dbOriginalList.Add(dbOriginal);
                }

                // Assign Literary Original to project
                if (project.LiteraryOriginals.All(x => x.Id != dbOriginal.Id))
                {
                    project.LiteraryOriginals.Add(dbOriginal);
                }
            }
        }

        private void UpdateLiteraryGenres(Project project)
        {
            project.LiteraryGenres.Clear();
            var dbGenreList = m_catalogValueRepository.GetLiteraryGenreList();

            foreach (var newGenreName in m_projectImportMetadata.Project.LiteraryGenres)
            {
                var dbGenre = dbGenreList.FirstOrDefault(x => x.Name == newGenreName);

                // Create new Literary Genre
                if (dbGenre == null)
                {
                    dbGenre = new LiteraryGenre
                    {
                        Name = newGenreName
                    };
                    m_catalogValueRepository.Create(dbGenre);
                    dbGenreList.Add(dbGenre);
                }

                // Assign Literary Genre to project
                if (project.LiteraryGenres.All(x => x.Id != dbGenre.Id))
                {
                    project.LiteraryGenres.Add(dbGenre);
                }
            }
        }

        private void UpdateAuthors(IList<ProjectOriginalAuthor> dbProjectAuthors)
        {
            var dbAuthors = dbProjectAuthors.Select(x => x.OriginalAuthor).ToList();
            var newAuthors =
                m_projectImportMetadata.Project.Authors.Select(x => new OriginalAuthor {FirstName = x.FirstName, LastName = x.LastName})
                    .ToList() ?? new List<OriginalAuthor>();

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
                m_projectRepository.Delete(projectAuthor);
            }


            for (var i = 0; i < newAuthors.Count; i++)
            {
                var newAuthor = newAuthors[i];
                if (authorsToAdd.Contains(newAuthor, comparer))
                {
                    var dbAuthor = GetOrCreateAuthor(newAuthor.FirstName, newAuthor.LastName);
                    var newProjectAuthor = new ProjectOriginalAuthor
                    {
                        OriginalAuthor = dbAuthor,
                        Project = m_projectRepository.Load<Project>(m_projectId),
                        Sequence = i + 1
                    };
                    m_projectRepository.Create(newProjectAuthor);
                }
                else
                {
                    var projectAuthor = dbProjectAuthors.Single(x =>
                        x.OriginalAuthor.FirstName == newAuthor.FirstName && x.OriginalAuthor.LastName == newAuthor.LastName);
                    projectAuthor.Sequence = i + 1;
                    m_projectRepository.Update(projectAuthor);
                }
            }
        }

        private OriginalAuthor GetOrCreateAuthor(string firstName, string lastName)
        {
            var dbAuthor = m_personRepository.GetAuthorByName(firstName, lastName);
            if (dbAuthor != null)
            {
                return dbAuthor;
            }

            var newDbAuthor = new OriginalAuthor
            {
                FirstName = firstName,
                LastName = lastName
            };
            m_projectRepository.Create(newDbAuthor);
            newDbAuthor = m_projectRepository.Load<OriginalAuthor>(newDbAuthor.Id);

            return newDbAuthor;
        }
    }
}