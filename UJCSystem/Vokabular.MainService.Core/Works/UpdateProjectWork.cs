using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works
{
    public class UpdateProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly ProjectContract m_data;
        private readonly int m_userId;

        public UpdateProjectWork(ProjectRepository projectRepository, long projectId, ProjectContract data, int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_data = data;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            //var now = DateTime.UtcNow;
            //var currentUser = m_projectRepository.Load<User>(m_userId);

            var project = m_projectRepository.FindById<Project>(m_projectId);

            if (project == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            project.Name = m_data.Name;
            
            m_projectRepository.Update(project);
        }
    }
}