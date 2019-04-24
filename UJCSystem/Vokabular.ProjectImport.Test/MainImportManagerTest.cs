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
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.OaiPmhImportManager;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Test.Mock;
using Vokabular.ProjectParsing.Model.Parsers;
using Vokabular.Shared.Const;

namespace Vokabular.ProjectImport.Test
{
    [TestClass]
    public class MainImportManagerTest
    {
        private MainImportManager m_mainImportManager;
        private ExternalRepositoryManager m_externalRepositoryManager;
        private FilteringExpressionSetManager m_filteringExpressionSetManager;
        private ImportHistoryManager m_importHistoryManager;
        private UserRepository m_userRepository;

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
                        target.Post(GetRecord("OaiPmh_Marc21_JanHus.xml"));
                        progressInfo.TotalProjectsCount = 1;
                    })
                .Returns(Task.CompletedTask);

            var mockIoc = new MockIocContainer(true);
            mockIoc.ServiceCollection.Replace(new ServiceDescriptor(typeof(IProjectImportManager),
                oaiPmhProjectImportManagerMock.Object));
            
            var serviceProvider = mockIoc.CreateServiceProvider();

            m_mainImportManager = serviceProvider.GetRequiredService<MainImportManager>();
            m_externalRepositoryManager = serviceProvider.GetRequiredService<ExternalRepositoryManager>();
            m_filteringExpressionSetManager = serviceProvider.GetRequiredService<FilteringExpressionSetManager>();
            m_importHistoryManager = serviceProvider.GetRequiredService<ImportHistoryManager>();
            m_userRepository = serviceProvider.GetRequiredService<UserRepository>();
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
            var userId = InitUser();
            var externalRepositoryId = InitRepository(userId);

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

            var importHistory = m_importHistoryManager.GetLatestSuccessfulImportHistory(externalRepositoryId);
            Assert.AreNotEqual(null, importHistory);
            Assert.AreEqual(ImportStatusEnum.Completed, importHistory.Status);
        }

        private recordType GetRecord(string name)
        {
            var xml = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + name);
            var oaiPmhRecord = xml.XmlDeserializeFromString<OAIPMHType>();
            return ((GetRecordType) oaiPmhRecord.Items.First()).record;
        }

        private int InitRepository(int userId)
        {
            var bibliographicFormatContract = new BibliographicFormatContract
            {
                Id = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new BibliographicFormat
                {
                    Name = BibliographicFormatNameConstant.Marc21
                }))
            };

            var externalRepositoryTypeId = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new ExternalRepositoryType
            {
                Name = ExternalRepositoryTypeNameConstant.OaiPhm
            }));

            var filterId = m_filteringExpressionSetManager.CreateFilteringExpressionSet(
                new FilteringExpressionSetDetailContract
                {
                    Name = "TestFilterSet",
                    BibliographicFormat = bibliographicFormatContract,
                    CreatedByUser = new UserContract {Id = userId},
                    FilteringExpressions = new List<FilteringExpressionContract>
                    {
                        new FilteringExpressionContract {Field = "100a", Value = "%Hus%"}
                    }
                }, userId);

            m_userRepository.InvokeUnitOfWork(x => x.Create(new BookType {Type = BookTypeEnum.BibliographicalItem}));

           return m_externalRepositoryManager.CreateExternalRepository(new ExternalRepositoryDetailContract
            {
                BibliographicFormat = bibliographicFormatContract,
                Configuration = "Configuration",
                Description = "Desc",
                ExternalRepositoryType = new ExternalRepositoryTypeContract {Id = externalRepositoryTypeId},
                Name = "RepoTest",
                Url = "test.cz",
                UrlTemplate = "test.cz/{0}",
                FilteringExpressionSets = new List<FilteringExpressionSetContract> {new FilteringExpressionSetContract {Id = filterId}}
            }, userId);

        }

        private int InitUser()
        {
             return (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new User
            {
                FirstName = "Test", LastName = "Test", Email = "Test@test.cz", AuthenticationProvider = AuthenticationProvider.ItJakub,
                CommunicationToken = "test", CreateTime = DateTime.UtcNow, UserName = "Test"
            }));
        }
    }
}