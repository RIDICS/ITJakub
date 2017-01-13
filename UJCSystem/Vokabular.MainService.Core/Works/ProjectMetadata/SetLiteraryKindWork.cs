using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetLiteraryKindWork : UnitOfWorkBase
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_kindIdList;

        public SetLiteraryKindWork(MetadataRepository metadataRepository, long projectId, IList<int> kindIdList) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_kindIdList = kindIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var literaryKindList = new List<LiteraryKind>();
            foreach (var id in m_kindIdList)
            {
                var literaryKind = m_metadataRepository.Load<LiteraryKind>(id);
                literaryKindList.Add(literaryKind);
            }

            var project = m_metadataRepository.Load<Project>(m_projectId);
            project.LiteraryKinds = literaryKindList;

            m_metadataRepository.Update(project);
        }
    }
}