using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.CreateProject;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class CreateOrUpdateProjectBasicDataWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long? m_nullableProjectId;
        private readonly int m_userId;
        private readonly BookData m_bookData;

        public CreateOrUpdateProjectBasicDataWork(ProjectRepository projectRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;

            m_nullableProjectId = resourceDirector.GetSessionInfoValue<long?>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_userId = resourceDirector.GetSessionInfoValue<int>(SessionInfo.UserId);
        }

        protected override long ExecuteWorkImplementation()
        {
            var updateProjectSubtask = new UpdateProjectSubtask(m_projectRepository);
            var projectId = updateProjectSubtask.UpdateProject(m_nullableProjectId, m_userId, m_bookData);

            return projectId;
        }
    }
}