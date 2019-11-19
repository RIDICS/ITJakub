using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class RemoveProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;

        public RemoveProjectWork(ProjectRepository projectRepository, long projectId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var project = m_projectRepository.FindById<Project>(m_projectId);

            if (project == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The project was not found.");
            }

            project.IsRemoved = true;

            m_projectRepository.Update(project);
        }
    }
}