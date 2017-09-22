using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetResponsiblePersonsWork : UnitOfWorkBase
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_responsiblePersonIdList;

        public SetResponsiblePersonsWork(MetadataRepository metadataRepository, long projectId, IList<int> responsiblePersonIdList) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_responsiblePersonIdList = responsiblePersonIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            throw new System.InvalidOperationException("Database model was changed. UI and logic update is required");

            var projectResponsiblePersonIdList = new List<ProjectResponsiblePersonIdContract>(); // TODO mock

            var project = m_metadataRepository.Load<Project>(m_projectId);
            var projectResponsiblePersonList = new List<ProjectResponsiblePerson>();

            // TODO load existing data and call Create, Update, Delete

            foreach (var projectPerson in projectResponsiblePersonIdList)
            {
                var responsiblePerson = m_metadataRepository.Load<ResponsiblePerson>(projectPerson.ResponsiblePersonId);
                var responsibleType = m_metadataRepository.Load<ResponsibleType>(projectPerson.ResponsibleTypeId);
                var newProjectResponsible = new ProjectResponsiblePerson
                {
                    Project = project,
                    ResponsiblePerson = responsiblePerson,
                    ResponsibleType = responsibleType,
                };

                m_metadataRepository.Create(newProjectResponsible);
            }

            
            //project.ResponsiblePersons = responsiblePersonList;

            m_metadataRepository.Update(project);
        }
    }
}