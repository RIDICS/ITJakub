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
        private readonly IList<ProjectResponsiblePersonIdContract> m_projectResponsiblePersonIdList;

        public SetResponsiblePersonsWork(MetadataRepository metadataRepository, long projectId, IList<ProjectResponsiblePersonIdContract> projectResponsiblePersonIdList) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_projectResponsiblePersonIdList = projectResponsiblePersonIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbProjectResponsibles = m_metadataRepository.GetProjectResponsibleList(m_projectId);
            var project = m_metadataRepository.Load<Project>(m_projectId);
            
            var newDbProjectResponsibles = new List<ProjectResponsiblePerson>();
            foreach (var projectPerson in m_projectResponsiblePersonIdList)
            {
                var responsiblePerson = m_metadataRepository.Load<ResponsiblePerson>(projectPerson.ResponsiblePersonId);
                var responsibleType = m_metadataRepository.Load<ResponsibleType>(projectPerson.ResponsibleTypeId);
                var newProjectResponsible = new ProjectResponsiblePerson
                {
                    Project = project,
                    ResponsiblePerson = responsiblePerson,
                    ResponsibleType = responsibleType,
                };

                newDbProjectResponsibles.Add(newProjectResponsible);
            }

            // Delete responsibles
            foreach (var dbProjectResponsible in dbProjectResponsibles)
            {
                if (!newDbProjectResponsibles.Contains(dbProjectResponsible))
                    m_metadataRepository.Delete(dbProjectResponsible);
            }

            // Create new responsibles
            foreach (var newDbProjectResponsible in newDbProjectResponsibles)
            {
                if (!dbProjectResponsibles.Contains(newDbProjectResponsible))
                    m_metadataRepository.Create(newDbProjectResponsible);
            }
        }
    }
}