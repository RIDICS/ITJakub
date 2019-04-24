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
    public class MockDataFactory
    {
        private readonly ExternalRepositoryManager m_externalRepositoryManager;
        private readonly FilteringExpressionSetManager m_filteringExpressionSetManager;
        private readonly ImportHistoryManager m_importHistoryManager;
        private readonly UserRepository m_userRepository;

        public MockDataFactory(ExternalRepositoryManager externalRepositoryManager, FilteringExpressionSetManager filteringExpressionSetManager, UserRepository userRepository, ImportHistoryManager importHistoryManager)
        {
            m_externalRepositoryManager = externalRepositoryManager;
            m_filteringExpressionSetManager = filteringExpressionSetManager;
            m_userRepository = userRepository;
            m_importHistoryManager = importHistoryManager;
        }

        public int BookTypeId { get; set; }
        public int UserId { get; set; }
        public int ExternalRepositoryId { get; set; }
        public int ImportHistoryId { get; set; }

        public int CreateExternalRepository(int userId)
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

            BookTypeId = (int)m_userRepository.InvokeUnitOfWork(x => x.Create(new BookType {Type = BookTypeEnum.BibliographicalItem}));

            ExternalRepositoryId = m_externalRepositoryManager.CreateExternalRepository(new ExternalRepositoryDetailContract
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

            return ExternalRepositoryId;
        }

        public int CreateUser()
        {
            UserId = (int) m_userRepository.InvokeUnitOfWork(x => x.Create(new User
            {
                FirstName = "Test", LastName = "Test", Email = "Test@test.cz", AuthenticationProvider = AuthenticationProvider.ItJakub,
                CommunicationToken = "test", CreateTime = DateTime.UtcNow, UserName = "Test"
            }));
            return UserId;
        }

        public int CreateImportHistory()
        {
            ImportHistoryId = m_importHistoryManager.CreateImportHistory(ExternalRepositoryId, UserId);
            return ImportHistoryId;
        }
    }
}