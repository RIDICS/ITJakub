using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class UpdateProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly string m_newName;

        public UpdateProjectWork(ProjectRepository projectRepository, long projectId, string newName) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_newName = newName;
        }

        protected override void ExecuteWorkImplementation()
        {
            var project = m_projectRepository.GetProject(m_projectId);

            if (project == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found."); 
            }

            project.Name = m_newName;

            m_projectRepository.Update(project);
        }
    }
}