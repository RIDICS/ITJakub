using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectWork : UnitOfWorkBase<Project>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly bool m_fetchPageCount;
        private readonly bool m_fetchAuthors;
        private readonly bool m_fetchResponsiblePersons;
        private readonly bool m_fetchBookTypes;
        private MetadataResource m_metadata;
        private int? m_pageCount;

        public GetProjectWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, long projectId, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons, bool fetchBookTypes) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_fetchPageCount = fetchPageCount;
            m_fetchAuthors = fetchAuthors;
            m_fetchResponsiblePersons = fetchResponsiblePersons;
            m_fetchBookTypes = fetchBookTypes;
        }

        protected override Project ExecuteWorkImplementation()
        {
            var dbResult = m_projectRepository.GetProject(m_projectId);
            m_metadata = m_metadataRepository
                .GetMetadataByProjectIds(new[] {m_projectId}, m_fetchAuthors, m_fetchResponsiblePersons, m_fetchBookTypes)
                .FirstOrDefault();

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