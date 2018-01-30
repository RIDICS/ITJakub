using ITJakub.FileProcessing.Core.Sessions.Works;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class BasicProjectDataRelationalDbStoreProcessor : IResourceProcessor
    {
        private readonly ProjectRepository m_projectRepository;

        public BasicProjectDataRelationalDbStoreProcessor(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var work = new CreateOrUpdateProjectBasicDataWork(m_projectRepository, resourceDirector);
            var projectId = work.Execute();

            resourceDirector.SetSessionInfoValue(SessionInfo.ProjectId, projectId);
        }
    }
}