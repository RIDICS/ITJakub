using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Data;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly UserManager m_userManager;

        public ProjectManager(ProjectRepository projectRepository, UserManager userManager)
        {
            m_projectRepository = projectRepository;
            m_userManager = userManager;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var currentUserId = m_userManager.GetCurrentUserId();
            var work = new CreateProjectWork(m_projectRepository, projectData, currentUserId);

            var resultId = work.Execute();
            return resultId;
        }

        public ProjectListData GetProjectList(int start, int count)
        {
            var work = new GetProjectListWork(m_projectRepository, start, count);
            var resultEntities = work.Execute();

            var result = new ProjectListData
            {
                List = Mapper.Map<List<ProjectContract>>(resultEntities),
                TotalCount = work.GetResultCount()
            };
            return result;
        }

        public ProjectContract GetProject(long projectId)
        {
            var work = new GetProjectWork(m_projectRepository, projectId);
            var resultEntity = work.Execute();

            var result = Mapper.Map<ProjectContract>(resultEntity);
            return result;
        }
    }
}
