using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectManager
    {
        private const int DefaultStartItem = 0;
        private const int DefaultProjectItemCount = 5;
        private const int MaxResultCount = 200;

        private readonly ProjectRepository m_projectRepository;
        private readonly UserManager m_userManager;

        public ProjectManager(ProjectRepository projectRepository, UserManager userManager)
        {
            m_projectRepository = projectRepository;
            m_userManager = userManager;
        }

        private int GetStart(int? start)
        {
            return start ?? DefaultStartItem;
        }

        private int GetCount(int? count)
        {
            return count != null ? Math.Min(count.Value, MaxResultCount) : DefaultProjectItemCount;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var currentUserId = m_userManager.GetCurrentUserId();
            var work = new CreateProjectWork(m_projectRepository, projectData, currentUserId);

            var resultId = work.Execute();
            return resultId;
        }

        public PagedResultList<ProjectContract> GetProjectList(int? start, int? count)
        {
            var startValue = GetStart(start);
            var countValue = GetCount(count);

            var work = new GetProjectListWork(m_projectRepository, startValue, countValue);
            var resultEntities = work.Execute();

            var result = new PagedResultList<ProjectContract>
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
