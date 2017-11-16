using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectWork : UnitOfWorkBase<Project>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly bool m_fetchPageCount;
        private MetadataResource m_metadata;
        private int? m_pageCount;

        public GetProjectWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, long projectId, bool fetchPageCount) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_fetchPageCount = fetchPageCount;
        }

        protected override Project ExecuteWorkImplementation()
        {
            var dbResult = m_projectRepository.GetProject(m_projectId);
            m_metadata = m_metadataRepository.GetLatestMetadataResource(m_projectId);

            if (m_fetchPageCount)
            {
                var pageCounts = m_metadataRepository.GetPageCount(new[] {m_projectId});
                var item = pageCounts.FirstOrDefault();
                if (item != null)
                {
                    m_pageCount = item.PageCount;
                }
            }

            return dbResult;
        }

        public MetadataResource GetMetadataResource()
        {
            return m_metadata;
        }

        public int? GetPageCount()
        {
            return m_pageCount;
        }
    }
}