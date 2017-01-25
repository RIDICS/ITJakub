using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetResponsiblePersonsWork : UnitOfWorkBase
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_responsiblePersonIdList;

        public SetResponsiblePersonsWork(MetadataRepository metadataRepository, long projectId, IList<int> responsiblePersonIdList) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_responsiblePersonIdList = responsiblePersonIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            throw new System.InvalidOperationException("Database model was changed. UI and logic update is required");
            var responsiblePersonList = new List<ResponsiblePerson>();
            foreach (var id in m_responsiblePersonIdList)
            {
                var responsiblePerson = m_metadataRepository.Load<ResponsiblePerson>(id);
                responsiblePersonList.Add(responsiblePerson);
            }

            var project = m_metadataRepository.Load<Project>(m_projectId);
            //project.ResponsiblePersons = responsiblePersonList;

            m_metadataRepository.Update(project);
        }
    }
}