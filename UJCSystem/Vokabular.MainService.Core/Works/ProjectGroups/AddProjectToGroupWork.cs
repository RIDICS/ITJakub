using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectGroups
{
    public class AddProjectToGroupWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_targetProjectId;
        private readonly long m_selectedProjectId;

        public AddProjectToGroupWork(ProjectRepository projectRepository, long targetProjectId, long selectedProjectId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_targetProjectId = targetProjectId;
            m_selectedProjectId = selectedProjectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var targetProjectGroup = m_projectRepository.GetProjectGroupWithProjects(m_targetProjectId);
            var currentProjectGroup = m_projectRepository.GetProjectGroupWithProjects(m_selectedProjectId);
            var currentGroupCount = currentProjectGroup?.Projects.Count;

            if (targetProjectGroup == null)
            {
                // Create new group

                var targetProject = m_projectRepository.FindById<Project>(m_targetProjectId);
                var selectedProject = m_projectRepository.FindById<Project>(m_selectedProjectId);

                var newProjectGroup = new ProjectGroup
                {
                    CreateTime = DateTime.UtcNow,
                    Projects = null,
                };

                targetProject.ProjectGroup = newProjectGroup;
                selectedProject.ProjectGroup = newProjectGroup;

                m_projectRepository.Create(newProjectGroup);
                m_projectRepository.Update(targetProject);
                m_projectRepository.Update(selectedProject);
            }
            else
            {
                // Update group

                var selectedProject = m_projectRepository.FindById<Project>(m_selectedProjectId);

                selectedProject.ProjectGroup = targetProjectGroup;

                m_projectRepository.Update(selectedProject);
            }


            if (currentGroupCount != null && currentGroupCount <= 1)
            {
                m_projectRepository.Delete(currentProjectGroup);
            }
        }
    }
}
