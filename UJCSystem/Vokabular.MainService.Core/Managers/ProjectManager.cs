using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly MetadataRepository m_metadataRepository;
        private readonly UserManager m_userManager;

        public ProjectManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, UserManager userManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
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

        public PagedResultList<ProjectDetailContract> GetProjectList(int? start, int? count, bool fetchPageCount)
        {
            var startValue = GetStart(start);
            var countValue = GetCount(count);

            var work = new GetProjectListWork(m_projectRepository, m_metadataRepository, startValue, countValue, fetchPageCount);
            var resultEntities = work.Execute();

            var metadataList = work.GetMetadataResources();
            var pageCountList = work.GetPageCountList();
            var resultList = Mapper.Map<List<ProjectDetailContract>>(resultEntities);
            foreach (var projectContract in resultList)
            {
                var metadataResource = metadataList.FirstOrDefault(x => x.Resource.Project.Id == projectContract.Id);
                var pageCountResult = pageCountList.FirstOrDefault(x => x.ProjectId == projectContract.Id);

                var metadataContract = Mapper.Map<ProjectMetadataContract>(metadataResource);
                projectContract.LatestMetadata = metadataContract;
                projectContract.PageCount = pageCountResult?.PageCount;
            }

            return new PagedResultList<ProjectDetailContract>
            {
                List = resultList,
                TotalCount = work.GetResultCount()
            };
        }

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, fetchPageCount);
            var project = work.Execute();

            if (project == null)
            {
                return null;
            }

            var result = Mapper.Map<ProjectDetailContract>(project);
            result.LatestMetadata = Mapper.Map<ProjectMetadataContract>(work.GetMetadataResource());
            result.PageCount = work.GetPageCount();

            return result;
        }
    }
}
