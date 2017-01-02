using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectManager
    {
        private readonly IUnitOfWork m_unitOfWork;
        private readonly ProjectRepository m_projectRepository;
        private readonly UserManager m_userManager;

        public ProjectManager(IUnitOfWork unitOfWork, ProjectRepository projectRepository, UserManager userManager)
        {
            m_unitOfWork = unitOfWork;
            m_projectRepository = projectRepository;
            m_userManager = userManager;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var work = new CreateProjectWork(m_unitOfWork, m_projectRepository, projectData, m_userManager);
            work.Execute();
            return work.GetResultId();
        }
    }
}
