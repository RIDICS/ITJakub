using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetResponsiblePersonsWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<ProjectResponsiblePersonIdContract> m_projectResponsiblePersonIdList;

        public SetResponsiblePersonsWork(ProjectRepository projectRepository, long projectId, IList<ProjectResponsiblePersonIdContract> projectResponsiblePersonIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_projectResponsiblePersonIdList = projectResponsiblePersonIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbProjectResponsibles = m_projectRepository.GetProjectResponsibleList(m_projectId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            
            var newDbProjectResponsibles = new List<ProjectResponsiblePerson>();
            foreach (var projectPerson in m_projectResponsiblePersonIdList)
            {
                var responsiblePerson = m_projectRepository.Load<ResponsiblePerson>(projectPerson.ResponsiblePersonId);
                var responsibleType = m_projectRepository.Load<ResponsibleType>(projectPerson.ResponsibleTypeId);
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
                    m_projectRepository.Delete(dbProjectResponsible);
            }

            // Create new responsibles
            foreach (var newDbProjectResponsible in newDbProjectResponsibles)
            {
                if (!dbProjectResponsibles.Contains(newDbProjectResponsible))
                    m_projectRepository.Create(newDbProjectResponsible);
            }
        }
    }
}