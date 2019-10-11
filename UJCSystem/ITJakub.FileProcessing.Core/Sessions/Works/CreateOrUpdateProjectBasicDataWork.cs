using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.CreateProject;
using ITJakub.FileProcessing.DataContracts;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class CreateOrUpdateProjectBasicDataWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long? m_nullableProjectId;
        private readonly int m_userId;
        private readonly BookData m_bookData;
        private readonly ProjectTypeContract m_projectType;

        public CreateOrUpdateProjectBasicDataWork(ProjectRepository projectRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;

            m_nullableProjectId = resourceDirector.GetSessionInfoValue<long?>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_userId = resourceDirector.GetSessionInfoValue<int>(SessionInfo.UserId);
            m_projectType = resourceDirector.GetSessionInfoValue<ProjectTypeContract>(SessionInfo.ProjectType);
        }

        protected override long ExecuteWorkImplementation()
        {
            var projectType = (ProjectTypeEnum) m_projectType;

            var updateProjectSubtask = new UpdateProjectSubtask(m_projectRepository);
            var projectId = updateProjectSubtask.UpdateProject(m_nullableProjectId, m_userId, m_bookData, projectType);

            return projectId;
        }
    }
}