using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.ProjectGroups;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectGroupManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly IMapper m_mapper;

        public ProjectGroupManager(ProjectRepository projectRepository, IMapper mapper)
        {
            m_projectRepository = projectRepository;
            m_mapper = mapper;
        }

        public ProjectGroupContract GetProjectGroups(long projectId)
        {
            var dbResult = m_projectRepository.InvokeUnitOfWork(x => x.GetProjectGroupWithProjects(projectId));
            var result = m_mapper.Map<ProjectGroupContract>(dbResult);
            return result;
        }

        public void AddProjectToGroup(long targetProjectId, long selectedProjectId)
        {
            var work = new AddProjectToGroupWork(m_projectRepository, targetProjectId, selectedProjectId);
            work.Execute();
        }

        public void RemoveProjectFromGroup(long projectId)
        {
            var work = new RemoveProjectFromGroupWork(m_projectRepository, projectId);
            work.Execute();
        }
    }
}