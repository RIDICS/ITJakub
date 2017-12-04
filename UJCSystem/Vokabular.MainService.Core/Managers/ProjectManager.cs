using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly UserManager m_userManager;

        public ProjectManager(ProjectRepository projectRepository, MetadataRepository metadataRepository, UserManager userManager)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_userManager = userManager;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var currentUserId = m_userManager.GetCurrentUserId();
            var work = new CreateProjectWork(m_projectRepository, projectData, currentUserId);

            var resultId = work.Execute();
            return resultId;
        }

        public void UpdateProject(long projectId, ProjectContract data)
        {
            var currentUserId = m_userManager.GetCurrentUserId();
            var work = new UpdateProjectWork(m_projectRepository, projectId, data, currentUserId);
            work.Execute();
        }

        public void DeleteProject(long projectId)
        {
            // TODO probably only set Project as removed
            throw new NotImplementedException();
        }

        public PagedResultList<ProjectDetailContract> GetProjectList(int? start, int? count, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCountForProject(count);

            var work = new GetProjectListWork(m_projectRepository, m_metadataRepository, startValue, countValue, fetchPageCount, fetchAuthors, fetchResponsiblePersons);
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

                if (fetchAuthors && metadataResource != null)
                    projectContract.Authors = Mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

                if (fetchResponsiblePersons && metadataResource != null)
                    projectContract.ResponsiblePersons = Mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);

            }

            return new PagedResultList<ProjectDetailContract>
            {
                List = resultList,
                TotalCount = work.GetResultCount()
            };
        }

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, fetchPageCount, fetchAuthors, fetchResponsiblePersons);
            var project = work.Execute();

            if (project == null)
            {
                return null;
            }

            var metadataResource = work.GetMetadataResource();
            var result = Mapper.Map<ProjectDetailContract>(project);
            result.LatestMetadata = Mapper.Map<ProjectMetadataContract>(metadataResource);
            result.PageCount = work.GetPageCount();

            if (fetchAuthors && metadataResource != null)
                result.Authors = Mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

            if (fetchResponsiblePersons && metadataResource != null)
                result.ResponsiblePersons = Mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);

            return result;
        }
    }
}
