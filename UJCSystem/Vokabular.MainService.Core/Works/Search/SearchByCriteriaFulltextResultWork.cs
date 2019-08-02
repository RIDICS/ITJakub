using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchByCriteriaFulltextResultWork : UnitOfWorkBase<IList<MetadataResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly FulltextSearchResultData m_fulltextSearchResultData;
        private readonly TermCriteriaPageConditionCreator m_termCriteriaCreator;

        public SearchByCriteriaFulltextResultWork(MetadataRepository metadataRepository, FulltextSearchResultData fulltextSearchResultData, TermCriteriaPageConditionCreator termCriteriaCreator) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_fulltextSearchResultData = fulltextSearchResultData;
            m_termCriteriaCreator = termCriteriaCreator;
        }

        protected override IList<MetadataResource> ExecuteWorkImplementation()
        {
            List<long> projectIds;
            IList<MetadataResource> metadataList;
            List<MetadataResource> orderedList;

            switch (m_fulltextSearchResultData.SearchResultType)
            {
                case FulltextSearchResultType.ProjectId:
                    projectIds = m_fulltextSearchResultData.LongList;
                    metadataList = m_metadataRepository.GetMetadataWithFetchForBiblModuleByProject(projectIds);
                    orderedList = CreateOrderedListById(projectIds, metadataList);
                    break;
                case FulltextSearchResultType.ProjectExternalId:
                    metadataList = m_metadataRepository.GetMetadataWithFetchForBiblModuleByProjectExternalIds(m_fulltextSearchResultData.StringList);
                    orderedList = CreateOrderedListByExternalId(m_fulltextSearchResultData.StringList, metadataList);
                    projectIds = metadataList.Select(x => x.Resource.Project.Id).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            PageCounts = m_metadataRepository.GetPageCount(projectIds);

            if (m_termCriteriaCreator != null)
            {
                m_termCriteriaCreator.SetProjectIds(projectIds);
                TermHits = projectIds.Count > 0
                    ? m_metadataRepository.GetPagesWithTerms(m_termCriteriaCreator)
                    : new List<PageResource>();
            }

            return orderedList;
        }

        private List<MetadataResource> CreateOrderedListById(IList<long> orderedProjectIdList, IList<MetadataResource> metadataList)
        {
            var result = new List<MetadataResource>();
            foreach (var projectId in orderedProjectIdList)
            {
                var metadataResource = metadataList.FirstOrDefault(x => x.Resource.Project.Id == projectId);
                result.Add(metadataResource);
            }
            return result;
        }

        private List<MetadataResource> CreateOrderedListByExternalId(IList<string> orderedProjectExternalIds, IList<MetadataResource> metadataList)
        {
            var result = new List<MetadataResource>();
            foreach (var projectExternalId in orderedProjectExternalIds)
            {
                var metadataResource = metadataList.FirstOrDefault(x => x.Resource.Project.ExternalId == projectExternalId);
                result.Add(metadataResource);
            }
            return result;
        }

        public IList<PageCountResult> PageCounts { get; set; }

        public IList<PageResource> TermHits { get; set; }
    }
}