using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectListWork : UnitOfWorkBase<IList<Project>>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly int m_start;
        private readonly int m_count;
        private readonly bool m_fetchPageCount;
        private readonly bool m_fetchAuthors;
        private readonly bool m_fetchResponsiblePersons;
        private int m_resultCount;
        private IList<MetadataResource> m_metadataList;
        private IList<PageCountResult> m_pageCount;

        public GetProjectListWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, int start, int count, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_start = start;
            m_count = count;
            m_fetchPageCount = fetchPageCount;
            m_fetchAuthors = fetchAuthors;
            m_fetchResponsiblePersons = fetchResponsiblePersons;
        }

        protected override IList<Project> ExecuteWorkImplementation()
        {
            var dbResult = m_projectRepository.GetProjectList(m_start, m_count);
            var projectIdList = dbResult.List.Select(x => x.Id).ToList();

            m_metadataList = m_metadataRepository.GetMetadataByProjectIds(projectIdList, m_fetchAuthors, m_fetchResponsiblePersons);
            m_resultCount = dbResult.Count;

            m_pageCount = m_fetchPageCount
                ? m_metadataRepository.GetPageCount(projectIdList)
                : new List<PageCountResult>();

            return dbResult.List;
        }

        public int GetResultCount()
        {
            return m_resultCount;
        }

        public IList<MetadataResource> GetMetadataResources()
        {
            return m_metadataList;
        }

        public IList<PageCountResult> GetPageCountList()
        {
            return m_pageCount;
        }
    }
}