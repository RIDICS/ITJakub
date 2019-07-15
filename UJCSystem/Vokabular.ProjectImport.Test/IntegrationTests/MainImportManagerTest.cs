using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.OaiPmhImportManager;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectImport.Test.Mock;
using Vokabular.ProjectParsing.Model.Parsers;

namespace Vokabular.ProjectImport.Test.IntegrationTests
{
    [TestClass]
    public class MainImportManagerTest
    {
        private MainImportManager m_mainImportManager;
        private ImportHistoryManager m_importHistoryManager;
        private MockDataConstant m_mockDataConstant;
        private MockDataManager m_mockDataManager;
        private ProjectRepository m_projectRepository;

        private const string ThrowExc = "ThrowException";
        private const string ImportTwoRecords = "ImportTwoRecords";

        [TestInitialize]
        public async Task Init()
        {
            var mockFactory = new MockRepository(MockBehavior.Loose) {CallBase = true};
            var oaiPmhProjectImportManagerMock = mockFactory.Create<OaiPmhProjectImportManager>(new object[1]);

            oaiPmhProjectImportManagerMock.Setup(x => x.ImportFromResource(
                    It.IsAny<string>(),
                    It.IsAny<ITargetBlock<object>>(),
                    It.IsAny<RepositoryImportProgressInfo>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, ITargetBlock<object>, RepositoryImportProgressInfo, DateTime?, CancellationToken>(
                    (config, target, progressInfo, dateTime, cancellationToken) =>
                    {
                        if (config == ThrowExc)
                        {
                            throw new ImportFailedException("ImportFailed");
                        }
                        else if (config == ImportTwoRecords)
                        {
                            target.Post(GetRecord(m_mockDataConstant.RecordOaiPmhMarc21JanHus));
                            target.Post(GetRecord(m_mockDataConstant.RecordOaiPmhMarc21JosefPekar));
                            progressInfo.TotalProjectsCount = 2;
                        }
                        else
                        {
                            target.Post(GetRecord(config));
                            progressInfo.TotalProjectsCount = 1;
                        }
                    })
                .Returns(Task.CompletedTask);

            var mockIoc = new MockIocContainer(true);
            mockIoc.ServiceCollection.Replace(new ServiceDescriptor(typeof(IProjectImportManager),
                oaiPmhProjectImportManagerMock.Object));

            var serviceProvider = mockIoc.CreateServiceProvider();

            m_mainImportManager = serviceProvider.GetRequiredService<MainImportManager>();
            m_importHistoryManager = serviceProvider.GetRequiredService<ImportHistoryManager>();
            m_mockDataConstant = serviceProvider.GetRequiredService<MockDataConstant>();
            m_mockDataManager = serviceProvider.GetRequiredService<MockDataManager>();
            m_projectRepository = serviceProvider.GetRequiredService<ProjectRepository>();

            var backgroundService = serviceProvider.GetService<IHostedService>();
            await backgroundService.StartAsync(CancellationToken.None);
        }

        [TestMethod]
        public void StartImportWithZeroRepositories()
        {
            Assert.ThrowsException<ArgumentNullException>(() => m_mainImportManager.ImportFromResources(new List<int>(), 1));
            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
        }

        [TestMethod]
        public void StartImportWithNullRepositories()
        {
            Assert.ThrowsException<ArgumentNullException>(() => m_mainImportManager.ImportFromResources(null, 1));
            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
        }

        [TestMethod]
        public void StartImportWithOneRepository()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository(m_mockDataConstant.RecordOaiPmhMarc21JanHus, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Hus%"}
            });

            m_mainImportManager.ImportFromResources(new List<int> {externalRepositoryId}, userId);

            Thread.Sleep(5000);

            while (m_mainImportManager.IsImportRunning)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
            Assert.AreEqual(1, m_mainImportManager.ActualProgress.Count);

            var info = m_mainImportManager.ActualProgress.First().Value;

            Assert.AreEqual(true, info.IsCompleted);
            Assert.AreEqual(1, info.TotalProjectsCount);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(1, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            var importHistory = m_importHistoryManager.GetLatestImportHistory(externalRepositoryId);
            Assert.AreNotEqual(null, importHistory);
            Assert.AreEqual(null, importHistory.Message);
            Assert.AreEqual(ImportStatusEnum.Completed, importHistory.Status);

            var projects = m_projectRepository.GetProjectList(0, 5);
            Assert.AreEqual(1, projects.Count);
        }

        [TestMethod]
        public void StartImportWithOneRepositoryTwoRecords()
        {
            var userId = m_mockDataManager.GetOrCreateUser();
            var externalRepositoryId = m_mockDataManager.CreateExternalRepository(ImportTwoRecords, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Hus%"},
                new FilteringExpressionContract {Field = "100a", Value = "%Josef%"}
            });

            m_mainImportManager.ImportFromResources(new List<int> {externalRepositoryId}, userId);

            Thread.Sleep(5000);

            while (m_mainImportManager.IsImportRunning)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
            Assert.AreEqual(1, m_mainImportManager.ActualProgress.Count);

            var info = m_mainImportManager.ActualProgress.First().Value;

            Assert.AreEqual(true, info.IsCompleted);
            Assert.AreEqual(2, info.TotalProjectsCount);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(2, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            var importHistory = m_importHistoryManager.GetLatestImportHistory(externalRepositoryId);
            Assert.AreNotEqual(null, importHistory);
            Assert.AreEqual(null, importHistory.Message);
            Assert.AreEqual(ImportStatusEnum.Completed, importHistory.Status);

            var projects = m_projectRepository.GetProjectList(0, 5);
            Assert.AreEqual(2, projects.Count);
        }

        [TestMethod]
        public void StartImportWithTwoRepositories()
        {
            var userId = m_mockDataManager.GetOrCreateUser();

            var externalRepositoryId1 = m_mockDataManager.CreateExternalRepository(m_mockDataConstant.RecordOaiPmhMarc21JanHus, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Hus%"}
            });

            var externalRepositoryId2 = m_mockDataManager.CreateExternalRepository(m_mockDataConstant.RecordOaiPmhMarc21JosefPekar, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Josef%"}
            });

            m_mainImportManager.ImportFromResources(new List<int> {externalRepositoryId1, externalRepositoryId2}, userId);

            Thread.Sleep(5000);

            while (m_mainImportManager.IsImportRunning)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
            Assert.AreEqual(2, m_mainImportManager.ActualProgress.Count);

            foreach (var info in m_mainImportManager.ActualProgress)
            {
                Assert.AreEqual(true, info.Value.IsCompleted);
                Assert.AreEqual(1, info.Value.TotalProjectsCount);
                Assert.AreEqual(0, info.Value.FailedProjectsCount);
                Assert.AreEqual(1, info.Value.ProcessedProjectsCount);
                Assert.AreEqual(null, info.Value.FaultedMessage);

                var importHistory = m_importHistoryManager.GetLatestImportHistory(info.Key);
                Assert.AreNotEqual(null, importHistory);
                Assert.AreEqual(null, importHistory.Message);
                Assert.AreEqual(ImportStatusEnum.Completed, importHistory.Status);
            }

            var projects = m_projectRepository.GetProjectList(0, 5);
            Assert.AreEqual(2, projects.Count);
        }

        [TestMethod]
        public void StartImportWithTwoRepositoriesOneFailed()
        {
            var userId = m_mockDataManager.GetOrCreateUser();

            var externalRepositoryId1 = m_mockDataManager.CreateExternalRepository(m_mockDataConstant.RecordOaiPmhMarc21JanHus, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Hus%"}
            });

            var externalRepositoryId2 = m_mockDataManager.CreateExternalRepository(ThrowExc, new List<FilteringExpressionContract>
            {
                new FilteringExpressionContract {Field = "100a", Value = "%Josef%"}
            });

            m_mainImportManager.ImportFromResources(new List<int> {externalRepositoryId1, externalRepositoryId2}, userId);

            Thread.Sleep(5000);

            while (m_mainImportManager.IsImportRunning)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(false, m_mainImportManager.IsImportRunning);
            Assert.AreEqual(2, m_mainImportManager.ActualProgress.Count);

            var info = m_mainImportManager.ActualProgress.First().Value;

            Assert.AreEqual(true, info.IsCompleted);
            Assert.AreEqual(1, info.TotalProjectsCount);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(1, info.ProcessedProjectsCount);
            Assert.AreEqual(null, info.FaultedMessage);

            info = m_mainImportManager.ActualProgress.First(x => x.Key == externalRepositoryId2).Value;

            Assert.AreEqual(true, info.IsCompleted);
            Assert.AreEqual(0, info.TotalProjectsCount);
            Assert.AreEqual(0, info.FailedProjectsCount);
            Assert.AreEqual(0, info.ProcessedProjectsCount);
            Assert.AreNotEqual(null, info.FaultedMessage);

            var importHistory = m_importHistoryManager.GetLatestImportHistory(externalRepositoryId1);
            Assert.AreNotEqual(null, importHistory);
            Assert.AreEqual(null, importHistory.Message);
            Assert.AreEqual(ImportStatusEnum.Completed, importHistory.Status);

            var importHistory2 = m_importHistoryManager.GetLatestImportHistory(externalRepositoryId2);
            Assert.AreNotEqual(null, importHistory2);
            Assert.AreNotEqual(null, importHistory2.Message);
            Assert.AreEqual(ImportStatusEnum.Failed, importHistory2.Status);

            var projects = m_projectRepository.GetProjectList(0, 5);
            Assert.AreEqual(1, projects.Count);
        }

        private recordType GetRecord(string name)
        {
            var xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), name));
            var oaiPmhRecord = xml.XmlDeserializeFromString<OAIPMHType>();
            return ((GetRecordType) oaiPmhRecord.Items.First()).record;
        }
    }
}