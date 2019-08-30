using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Test.Mock;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Test.UnitTests
{
    [TestClass]
    public class SaveImportedDataWorkTest
    {
        private MockDataManager m_mockDataManager;
        private ProjectRepository m_projectRepository;
        private MetadataRepository m_metadataRepository;
        private PersonRepository m_personRepository;
        private PermissionRepository m_permissionRepository;
        private CatalogValueRepository m_catalogValueRepository;
        private ExternalRepositoryRepository m_externalRepositoryRepository;
        private ImportedProjectMetadataManager m_importedProjectMetadataManager;
        private ImportedRecord m_importedRecord;

        [TestInitialize]
        public void Init()
        {
            var mockIoc = new MockIocContainer(true);
            var serviceProvider = mockIoc.CreateServiceProvider();

            m_mockDataManager = serviceProvider.GetRequiredService<MockDataManager>();
            m_projectRepository = serviceProvider.GetRequiredService<ProjectRepository>();
            m_metadataRepository = serviceProvider.GetRequiredService<MetadataRepository>();
            m_personRepository = serviceProvider.GetRequiredService<PersonRepository>();
            m_permissionRepository = serviceProvider.GetRequiredService<PermissionRepository>();
            m_catalogValueRepository = serviceProvider.GetRequiredService<CatalogValueRepository>();
            m_externalRepositoryRepository = serviceProvider.GetRequiredService<ExternalRepositoryRepository>();
            m_importedProjectMetadataManager = serviceProvider.GetRequiredService<ImportedProjectMetadataManager>();

            m_importedRecord = new ImportedRecord
            {
                IsNew = true,
                IsFailed = false,
                IsDeleted = false,
                ImportedProject = new ImportedProject
                {
                    Id = "1",
                    ProjectMetadata = new ProjectMetadata
                    {
                        Title = "Title",
                        PublisherText = "PublisherText",
                        PublishDate = "PublishDate",
                        PublishPlace = "PublishPlace",
                    },
                    Authors = new HashSet<Author> {new Author("Jan", "Hus")},
                    Keywords = new List<string> {"Keyword"},
                    LiteraryGenres = new List<string> {"LiteraryGenre"}
                },
                ExternalId = "Ext1"
            };
        }

        [TestMethod]
        public void SaveFullRecord()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());
            
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, m_importedRecord, userId, externalRepositoryId,
                m_mockDataManager.GetOrCreateBookType(),
                new List<int>()).Execute();

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);

            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, true, false, true, true, true,
                false);

            var externalRepository = m_externalRepositoryRepository.GetExternalRepository(externalRepositoryId);

            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, project.Name);
            Assert.AreEqual(string.Format(externalRepository.UrlTemplate, m_importedRecord.ImportedProject.Id), project.OriginalUrl);
            Assert.AreEqual(userId, project.CreatedByUser.Id);
            Assert.AreEqual(1, project.Snapshots.Count);

            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.Keywords.ToList(), project.Keywords.Select(x => x.Text).ToList());
            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.LiteraryGenres.ToList(),
                project.LiteraryGenres.Select(x => x.Name).ToList());

            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.Count, project.Authors.Count);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().FirstName, project.Authors.First().OriginalAuthor.FirstName);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().LastName, project.Authors.First().OriginalAuthor.LastName);

            var metadataResource = m_metadataRepository.GetLatestMetadataResource(m_importedRecord.ProjectId);

            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, metadataResource.Title);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublisherText, metadataResource.PublisherText);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishDate, metadataResource.PublishDate);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace, metadataResource.PublishPlace);

            var importedProjectMetadata = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(m_importedRecord.ExternalId);

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);
        }

        [TestMethod]
        public void UpdateFullRecordAddInformation()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());

            //Create project
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, m_importedRecord, userId, externalRepositoryId,
                m_mockDataManager.GetOrCreateBookType(),
                new List<int>()).Execute();

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);
            var projectId = m_importedRecord.ProjectId;

            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, false, false, true, true,
                true,
                false);

            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.Keywords.ToList(), project.Keywords.Select(x => x.Text).ToList());
            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.LiteraryGenres.ToList(),
                project.LiteraryGenres.Select(x => x.Name).ToList());

            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.Count, project.Authors.Count);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().FirstName, project.Authors.First().OriginalAuthor.FirstName);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().LastName, project.Authors.First().OriginalAuthor.LastName);

            var metadataResource = m_metadataRepository.GetLatestMetadataResource(m_importedRecord.ProjectId);

            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, metadataResource.Title);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublisherText, metadataResource.PublisherText);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishDate, metadataResource.PublishDate);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace, metadataResource.PublishPlace);

            var importedProjectMetadata = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(m_importedRecord.ExternalId);

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);


            //Update project
            m_importedRecord.IsNew = false;
            m_importedRecord.ImportedProject.ProjectMetadata.Title = "UpdatedTitle";
            m_importedRecord.ImportedProject.ProjectMetadata.PublisherText = "UpdatedPublisherText";
            m_importedRecord.ImportedProject.Keywords.Add("Keyword2");
            m_importedRecord.ImportedProject.LiteraryGenres.Add("LiteraryGenre2");
            m_importedRecord.ImportedProject.Authors.Add(new Author("FirstName2", "LastName2"));

            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, m_importedRecord, userId, externalRepositoryId,
                m_mockDataManager.GetOrCreateBookType(),
                new List<int>()).Execute();

            Assert.AreEqual(projectId, m_importedRecord.ProjectId);

            project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, false, false, true, true,
                true,
                false);

            Assert.AreEqual(2, project.Snapshots.Count);

            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.Keywords.ToList(), project.Keywords.Select(x => x.Text).ToList());
            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.LiteraryGenres.ToList(),
                project.LiteraryGenres.Select(x => x.Name).ToList());

            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.Count, project.Authors.Count);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().FirstName, project.Authors.First().OriginalAuthor.FirstName);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().LastName, project.Authors.First().OriginalAuthor.LastName);

            metadataResource = m_metadataRepository.GetLatestMetadataResource(m_importedRecord.ProjectId);

            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, metadataResource.Title);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublisherText, metadataResource.PublisherText);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishDate, metadataResource.PublishDate);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace, metadataResource.PublishPlace);

            importedProjectMetadata = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(m_importedRecord.ExternalId);

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);
        }

        [TestMethod]
        public void UpdateFullRecordReplaceInformation()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());

            //Create project
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, m_importedRecord, userId, externalRepositoryId,
                m_mockDataManager.GetOrCreateBookType(),
                new List<int>()).Execute();

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);
            var projectId = m_importedRecord.ProjectId;

            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, false, false, true, true,
                true,
                false);

            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.Keywords.ToList(), project.Keywords.Select(x => x.Text).ToList());
            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.LiteraryGenres.ToList(),
                project.LiteraryGenres.Select(x => x.Name).ToList());

            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.Count, project.Authors.Count);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().FirstName, project.Authors.First().OriginalAuthor.FirstName);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().LastName, project.Authors.First().OriginalAuthor.LastName);

            var metadataResource = m_metadataRepository.GetLatestMetadataResource(m_importedRecord.ProjectId);

            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, metadataResource.Title);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublisherText, metadataResource.PublisherText);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishDate, metadataResource.PublishDate);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace, metadataResource.PublishPlace);

            var importedProjectMetadata = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(m_importedRecord.ExternalId);

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);


            //Update project
            m_importedRecord.IsNew = false;
            m_importedRecord.ImportedProject.ProjectMetadata.Title = "UpdatedTitle";
            m_importedRecord.ImportedProject.ProjectMetadata.PublisherText = "UpdatedPublisherText";
            m_importedRecord.ImportedProject.Keywords = new List<string>{"Keyword2"};
            m_importedRecord.ImportedProject.LiteraryGenres = new List<string> {"LiteraryGenre2"};

            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, m_importedRecord, userId, externalRepositoryId,
                m_mockDataManager.GetOrCreateBookType(),
                new List<int>()).Execute();

            Assert.AreEqual(projectId, m_importedRecord.ProjectId);

            project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, false, false, true, true,
                true,
                false);

            Assert.AreEqual(2, project.Snapshots.Count);

            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.Keywords.ToList(), project.Keywords.Select(x => x.Text).ToList());
            CollectionAssert.AreEquivalent(m_importedRecord.ImportedProject.LiteraryGenres.ToList(),
                project.LiteraryGenres.Select(x => x.Name).ToList());

            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.Count, project.Authors.Count);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().FirstName, project.Authors.First().OriginalAuthor.FirstName);
            Assert.AreEqual(m_importedRecord.ImportedProject.Authors.First().LastName, project.Authors.First().OriginalAuthor.LastName);

            metadataResource = m_metadataRepository.GetLatestMetadataResource(m_importedRecord.ProjectId);
            
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.Title, metadataResource.Title);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublisherText, metadataResource.PublisherText);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishDate, metadataResource.PublishDate);
            Assert.AreEqual(m_importedRecord.ImportedProject.ProjectMetadata.PublishPlace, metadataResource.PublishPlace);

            importedProjectMetadata = m_importedProjectMetadataManager.GetImportedProjectMetadataByExternalId(m_importedRecord.ExternalId);

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);
        }
    }
}