using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Managers;
using Vokabular.Shared.Const;

namespace Vokabular.ProjectImport.Test.Mock
{
    public class MockDataManager
    {
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly ImportHistoryManager m_importHistoryManager;
        private readonly UserRepository m_userRepository;

        public MockDataManager(ExternalRepositoryManager externalRepositoryManager,
            FilteringExpressionSetManager filteringExpressionSetManager, UserRepository userRepository,
            ImportHistoryManager importHistoryManager)
        {
            m_externalRepositoryManager = externalRepositoryManager;
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_userRepository = userRepository;
            m_importHistoryManager = importHistoryManager;
        }

        private int? m_bookTypeId;
        private int? m_userId;
        private int m_externalRepositoryId;
        private BibliographicFormatContract m_bibliographicFormatContract;
        private ExternalRepositoryTypeContract m_externalRepositoryTypeContract;

        private BibliographicFormatContract GetOrCreateBibliographicFormatContract()
        {
            return m_bibliographicFormatContract ?? (m_bibliographicFormatContract = new BibliographicFormatContract
            {
                Id = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new BibliographicFormat
                {
                    Name = BibliographicFormatNameConstant.Marc21
                }))
            });
        }

        private ExternalRepositoryTypeContract GetOrCreateExternalRepositoryTypeContract()
        {
            return m_externalRepositoryTypeContract ?? (m_externalRepositoryTypeContract = new ExternalRepositoryTypeContract
            {
                Id = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new ExternalRepositoryType
                {
                    Name = ExternalRepositoryTypeNameConstant.OaiPhm
                }))
            });
        }
        
        public int GetOrCreateBookType()
        {
            if (!m_bookTypeId.HasValue)
            {
                m_bookTypeId = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new BookType {Type = BookTypeEnum.BibliographicalItem}));
            }

            return m_bookTypeId.Value;
        }

        private int CreateFilter(IList<FilteringExpressionContract> filteringExpressionContractList)
        {
            return m_filteringExpressionSetManager.CreateFilteringExpressionSet(
                new FilteringExpressionSetDetailContract
                {
                    Name = "TestFilterSet" + DateTime.UtcNow,
                    BibliographicFormat = GetOrCreateBibliographicFormatContract(),
                    CreatedByUser = new UserContract {Id = GetOrCreateUser()},
                    FilteringExpressions = filteringExpressionContractList
                }, GetOrCreateUser());
        }

        public int CreateExternalRepository(string configuration, IList<FilteringExpressionContract> filteringExpressionContractList)
        {
            GetOrCreateBookType();
            var filterId = CreateFilter(filteringExpressionContractList);

            m_externalRepositoryId = m_externalRepositoryManager.CreateExternalRepository(new ExternalRepositoryDetailContract
            {
                BibliographicFormat = GetOrCreateBibliographicFormatContract(),
                Configuration = configuration,
                Description = "Desc",
                ExternalRepositoryType = GetOrCreateExternalRepositoryTypeContract(),
                Name = "RepoTest" + DateTime.UtcNow,
                Url = "test.cz",
                UrlTemplate = "test.cz/{0}",
                FilteringExpressionSets = new List<FilteringExpressionSetContract> {new FilteringExpressionSetContract {Id = filterId}}
            }, GetOrCreateUser());

            return m_externalRepositoryId;
        }

        public int GetOrCreateUser()
        {
            if (!m_userId.HasValue)
            {
                m_userId = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new User
                {
                    FirstName = "Test", LastName = "Test", Email = "Test@test.cz", AuthenticationProvider = AuthenticationProvider.ItJakub,
                    CommunicationToken = "test", CreateTime = DateTime.UtcNow, UserName = "Test"
                }));
            }

            return m_userId.Value;
        }

        public int CreateImportHistory()
        {
            return m_importHistoryManager.CreateImportHistory(m_externalRepositoryId, GetOrCreateUser());
        }
    }
}