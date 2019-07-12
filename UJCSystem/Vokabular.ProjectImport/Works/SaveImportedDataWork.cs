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
        private readonly ImportedRecord m_importedRecord;
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_userId;
        private readonly int m_externalRepositoryId;
        private long m_projectId;
        private readonly int m_bookTypeId;
        private readonly IList<int> m_roleIds;


        public SaveImportedDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            CatalogValueRepository catalogValueRepository, PersonRepository personRepository, PermissionRepository permissionRepository,
            ImportedRecord importedRecord, int userId, int externalRepositoryId, int bookTypeId, IList<int> roleIds) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_permissionRepository = permissionRepository;
            m_importedRecord = importedRecord;
            m_userId = userId;
            m_externalRepositoryId = externalRepositoryId;
            m_bookTypeId = bookTypeId;
            m_roleIds = roleIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            if (m_importedRecord.IsNew)
            {
                m_importedRecord.ProjectId = CreateProject();
                m_importedRecord.ImportedProjectMetadataId = CreateImportedProjectMetadata();
            }

            m_projectId = m_importedRecord.ProjectId;
            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_projectId, true, false, false, true, true, true, false);

            UpdateLiteraryGenres(project);
            UpdateLiteraryOriginals(project);
            UpdateKeywords(project);
            UpdateAuthors(project);
            UpdateMetadata(project);

            m_catalogValueRepository.Update(project);

            CreateSnapshot();
            ProcessExternalImportPermission();
            m_projectRepository.UnitOfWork.CurrentSession.Evict(project); //because of unit tests - unit test is running in one session
        }

        private int CreateImportedProjectMetadata()
        {
            var project = m_projectRepository.Load<Project>(m_importedRecord.ProjectId);
            var externalRepository = m_projectRepository.Load<ExternalRepository>(m_externalRepositoryId);

            var importedProjectMetadata = new ImportedProjectMetadata
            {
                ExternalId = m_importedRecord.ExternalId,
                Project = project,
                ExternalRepository = externalRepository
            };

            return (int)m_projectRepository.Create(importedProjectMetadata);
        }

        private long CreateProject()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var externalRepository = m_projectRepository.Load<ExternalRepository>(m_externalRepositoryId);

            var project = new Project
            {
                Name = m_importedRecord.ImportedProject.ProjectMetadata.Title,
                CreateTime = now,
                CreatedByUser = user,
                OriginalUrl = string.Format(externalRepository.UrlTemplate, m_importedRecord.ImportedProject.Id)
            };

            return (long)m_projectRepository.Create(project);
        }

        private void UpdateMetadata(Project project)
        {
            var now = DateTime.UtcNow;
            var lastMetadata = m_metadataRepository.GetLatestMetadataResource(m_projectId);
            var firstManuscript = m_importedRecord.ImportedProject.ProjectMetadata.ManuscriptDescriptionData;

            var authorsString = m_importedRecord.ImportedProject.Authors != null
                ? string.Join(", ", m_importedRecord.ImportedProject.Authors.Select(x => $"{x.LastName} {x.FirstName}"))
                : null;

            var projectMetadata = m_importedRecord.ImportedProject.ProjectMetadata;
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

        private void UpdateKeywords(Project project)
        {
            if (project.Keywords == null)
            {
                project.Keywords = new List<Keyword>();
            }
            else
            {
                project.Keywords.Clear();
            }

            foreach (var newKeywordName in m_importedRecord.ImportedProject.Keywords)
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
            if (project.LiteraryOriginals == null)
            {
                project.LiteraryOriginals = new List<LiteraryOriginal>();
            }
            else
            {
                project.LiteraryOriginals.Clear();
            }
            
            var dbOriginalList = m_catalogValueRepository.GetLiteraryOriginalList();

            foreach (var newOriginalName in m_importedRecord.ImportedProject.LiteraryOriginals)
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
            if (project.LiteraryGenres == null)
            {
                project.LiteraryGenres = new List<LiteraryGenre>();
            }
            else
            {
                project.LiteraryGenres.Clear();
            }
            
            var dbGenreList = m_catalogValueRepository.GetLiteraryGenreList();

            foreach (var newGenreName in m_importedRecord.ImportedProject.LiteraryGenres)
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

        private void UpdateAuthors(Project project)
        {
            if (project.Authors == null)
            {
                project.Authors = new List<ProjectOriginalAuthor>();
            }

            var dbAuthors = project.Authors.Select(x => x.OriginalAuthor).ToList();
            var newAuthors =
                m_importedRecord.ImportedProject.Authors.Select(x => new OriginalAuthor {FirstName = x.FirstName, LastName = x.LastName})
                    .ToList();

            var comparer = new AuthorNameEqualityComparer();
            var authorsToAdd = newAuthors.Except(dbAuthors, comparer).ToList();
            var authorsToRemove = dbAuthors.Except(newAuthors, comparer).ToList();

            if (authorsToAdd.Count == 0 && authorsToRemove.Count == 0)
            {
                return;
            }


            foreach (var author in authorsToRemove)
            {
                var projectAuthor = project.Authors.Single(x => x.OriginalAuthor.Id == author.Id);
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
                    var projectAuthor = project.Authors.Single(x =>
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

        private void CreateSnapshot()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);
            var dbBookType = m_projectRepository.Load<BookType>(m_bookTypeId);

            var versionNumber = latestSnapshot?.VersionNumber ?? 0;
            var newDbSnapshot = new Snapshot
            {
                Project = project,
                BookTypes = new List<BookType> { dbBookType },
                DefaultBookType = dbBookType,
                CreateTime = now,
                PublishTime = now,
                CreatedByUser = user,
                VersionNumber = versionNumber + 1
            };

            m_projectRepository.Create(newDbSnapshot);

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);
        }

        private void ProcessExternalImportPermission()
        {
            var project = m_permissionRepository.Load<Project>(m_projectId);

            var newPermissions = m_roleIds.Select(groupId => new Permission
            {
                Project = project,
                UserGroup = m_projectRepository.Load<UserGroup>(groupId)
            });

            foreach (var newPermission in newPermissions)
            {
                m_permissionRepository.CreatePermissionIfNotExist(newPermission);
            }
        }
    }
}