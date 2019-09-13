using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.ExternalBibliography;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Test.Mock;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Test.IntegrationTests
{
    [TestClass]
    public class ImportedProjectManagerTest
    {
        private MockDataManager m_mockDataManager;
        private ProjectRepository m_projectRepository;
        private MetadataRepository m_metadataRepository;
        private ImportedRecordMetadataRepository m_importedRecordMetadataRepository;
        private ImportedProjectMetadataRepository m_importedProjectMetadataRepository;
        private ImportedProjectManager m_importedProjectManager;
        private ImportedRecord m_importedRecord;

        [TestInitialize]
        public void Init()
        {
            var mockIoc = new MockIocContainer(true);
            var serviceProvider = mockIoc.CreateServiceProvider();

            m_mockDataManager = serviceProvider.GetRequiredService<MockDataManager>();
            m_projectRepository = serviceProvider.GetRequiredService<ProjectRepository>();
            m_metadataRepository = serviceProvider.GetRequiredService<MetadataRepository>();
            m_importedRecordMetadataRepository = serviceProvider.GetRequiredService<ImportedRecordMetadataRepository>();
            m_importedProjectMetadataRepository = serviceProvider.GetRequiredService<ImportedProjectMetadataRepository>();
            m_importedProjectManager = serviceProvider.GetRequiredService<ImportedProjectManager>();

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
        public void SaveImportedRecord()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());
            var historyId = m_mockDataManager.CreateImportHistory();
            var info = new RepositoryImportProgressInfo(externalRepositoryId, "Test");

             m_importedProjectManager.SaveImportedProject(m_importedRecord, userId, externalRepositoryId, m_mockDataManager.GetOrCreateBookType(),
                new List<int>(), historyId, info);

            Assert.AreEqual(false, info.IsCompleted);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(1, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);

            var project = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, true, false, true, true, true,
                false);
           
            var snapshot = m_projectRepository.GetLatestSnapshot(m_importedRecord.ProjectId);
            var importedRecordMetadata = m_importedRecordMetadataRepository.InvokeUnitOfWork(x => x.GetImportedRecordMetadataBySnapshot(snapshot.Id));
            var importedProjectMetadata = m_importedProjectMetadataRepository.InvokeUnitOfWork(x => x.GetImportedProjectMetadata(m_importedRecord.ExternalId));

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(project.Id, importedProjectMetadata.Project.Id);

            Assert.AreEqual(historyId, importedRecordMetadata.LastUpdate.Id);
            Assert.AreEqual(null, importedRecordMetadata.LastUpdateMessage);
            Assert.AreEqual(importedProjectMetadata.Id, importedRecordMetadata.ImportedProjectMetadata.Id);
        }

        [TestMethod]
        public void UpdateImportedRecord()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());
            var historyId = m_mockDataManager.CreateImportHistory();
            var info = new RepositoryImportProgressInfo(externalRepositoryId, "Test");

            m_importedProjectManager.SaveImportedProject(m_importedRecord, userId, externalRepositoryId, m_mockDataManager.GetOrCreateBookType(),
                new List<int>(), historyId, info);

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);

            //Update 
            m_importedRecord.IsNew = false;
            m_importedRecord.ImportedProject.ProjectMetadata.Title = "UpdatedTitle";

            var historyId2 = m_mockDataManager.CreateImportHistory();
            m_importedProjectManager.SaveImportedProject(m_importedRecord, userId, externalRepositoryId, m_mockDataManager.GetOrCreateBookType(),
                new List<int>(), historyId2, info);

            Assert.AreEqual(false, info.IsCompleted);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(2, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            Assert.AreNotEqual(null, m_importedRecord.ProjectId);

            var updatedProject = m_metadataRepository.GetAdditionalProjectMetadata(m_importedRecord.ProjectId, true, true, false, true, true, true,
                false);
           
            var newSnapshot = m_projectRepository.GetLatestSnapshot(m_importedRecord.ProjectId);
            var newImportedRecordMetadata = m_importedRecordMetadataRepository.InvokeUnitOfWork(x => x.GetImportedRecordMetadataBySnapshot(newSnapshot.Id));
            var importedProjectMetadata = m_importedProjectMetadataRepository.InvokeUnitOfWork(x => x.GetImportedProjectMetadata(m_importedRecord.ExternalId));

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(updatedProject.Id, importedProjectMetadata.Project.Id);

            Assert.AreEqual(historyId2, newImportedRecordMetadata.LastUpdate.Id);
            Assert.AreEqual(null, newImportedRecordMetadata.LastUpdateMessage);
            Assert.AreEqual(importedProjectMetadata.Id, newImportedRecordMetadata.ImportedProjectMetadata.Id);
        }

        [TestMethod]
        public void SaveFailedImportedRecord()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository("", new List<FilteringExpressionContract>());
           
            var info = new RepositoryImportProgressInfo(externalRepositoryId, "Test");

            m_importedProjectManager.SaveImportedProject(m_importedRecord, userId, externalRepositoryId, m_mockDataManager.GetOrCreateBookType(),
                new List<int>(), m_mockDataManager.CreateImportHistory(), info);

            m_importedRecord.IsNew = false;
            m_importedRecord.IsFailed = true;
            m_importedRecord.ImportedProject = null;
            m_importedRecord.FaultedMessage = "Message";

            var historyId = m_mockDataManager.CreateImportHistory();

            m_importedProjectManager.SaveImportedProject(m_importedRecord, userId, externalRepositoryId, m_mockDataManager.GetOrCreateBookType(),
                new List<int>(), historyId, info);

            Assert.AreEqual(false, info.IsCompleted);
            Assert.AreEqual(1, info.FailedProjectsCount);
            Assert.AreEqual(2, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            var importedProjectMetadata = m_importedProjectMetadataRepository.InvokeUnitOfWork(x => x.GetImportedProjectMetadata(m_importedRecord.ExternalId));

            Assert.AreNotEqual(null, importedProjectMetadata.Id);
            Assert.AreEqual(m_importedRecord.ExternalId, importedProjectMetadata.ExternalId);
            Assert.AreEqual(true, m_importedRecord.ImportedProjectMetadataId.HasValue);

            var importedRecordMetadata = m_importedRecordMetadataRepository.InvokeUnitOfWork(x => x.GetImportedRecordMetadataByImportedProjectMetadata(m_importedRecord.ImportedProjectMetadataId.Value));
            var metadata = importedRecordMetadata.FirstOrDefault(x => x.Snapshot == null);

            Assert.AreNotEqual(null, metadata);
            Assert.AreEqual(historyId, metadata.LastUpdate.Id);
            Assert.AreEqual(m_importedRecord.FaultedMessage, metadata.LastUpdateMessage);
            Assert.AreEqual(importedProjectMetadata.Id, metadata.ImportedProjectMetadata.Id);
        }
    }
}