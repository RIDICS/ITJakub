using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Results;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;
        private readonly CommunicationProvider m_communicationProvider;

        public ProjectManager(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            PermissionRepository permissionRepository, AuthenticationManager authenticationManager,
            UserDetailManager userDetailManager, CommunicationProvider communicationProvider)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_permissionRepository = permissionRepository;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_communicationProvider = communicationProvider;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var currentUserId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateProjectWork(m_projectRepository, projectData, currentUserId);

            var resultId = work.Execute();
            return resultId;
        }

        public void UpdateProject(long projectId, ProjectContract data)
        {
            var currentUserId = m_authenticationManager.GetCurrentUserId();
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
                projectContract.CreatedByUser = m_userDetailManager.GetUserContractForUser(projectContract.CreatedByUser);

                if (fetchAuthors && metadataResource != null)
                    projectContract.Authors = Mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

                if (fetchResponsiblePersons && metadataResource != null)
                    projectContract.ResponsiblePersons =
                        Mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);
            }

            return new PagedResultList<ProjectDetailContract>
            {
                List = resultList,
                TotalCount = work.GetResultCount()
            };
        }

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, fetchPageCount, fetchAuthors,
                fetchResponsiblePersons);
            var project = work.Execute();

            if (project == null)
            {
                return null;
            }

            var metadataResource = work.GetMetadataResource();
            var result = Mapper.Map<ProjectDetailContract>(project);
            result.CreatedByUser = m_userDetailManager.GetUserContractForUser(result.CreatedByUser);
            result.LatestMetadata = Mapper.Map<ProjectMetadataContract>(metadataResource);
            result.PageCount = work.GetPageCount();

            if (fetchAuthors && metadataResource != null)
                result.Authors = Mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

            if (fetchResponsiblePersons && metadataResource != null)
                result.ResponsiblePersons =
                    Mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);

            return result;
        }

        private List<ProjectDetailContract> MapProjectsToContractList(IEnumerable<MetadataResource> dbMetadataList)
        {
            var resultList = new List<ProjectDetailContract>();
            foreach (var metadataResource in dbMetadataList)
            {
                var project = metadataResource.Resource.Project;
                var resultItem = Mapper.Map<ProjectDetailContract>(project);
                resultItem.CreatedByUser = m_userDetailManager.GetUserContractForUser(resultItem.CreatedByUser);
                resultItem.LatestMetadata = Mapper.Map<ProjectMetadataContract>(metadataResource);
                resultItem.Authors = Mapper.Map<List<OriginalAuthorContract>>(project.Authors);
                resultItem.ResponsiblePersons = Mapper.Map<List<ProjectResponsiblePersonContract>>(project.ResponsiblePersons);

                resultList.Add(resultItem);
            }

            return resultList;
        }

        private PagedResultList<ProjectDetailContract> MapPagedProjectsToContractList(ListWithTotalCountResult<MetadataResource> dbResult)
        {
            var resultContractList = MapProjectsToContractList(dbResult.List);

            return new PagedResultList<ProjectDetailContract>
            {
                List = resultContractList,
                TotalCount = dbResult.Count,
            };
        }

        public PagedResultList<ProjectDetailContract> GetProjectsByAuthor(int authorId, int? start, int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByAuthor(authorId, startValue, countValue));
            var result = MapPagedProjectsToContractList(dbMetadataList);

            return result;
        }

        public PagedResultList<ProjectDetailContract> GetProjectsByResponsiblePerson(int responsiblePersonId, int? start, int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var dbMetadataList =
                m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByResponsiblePerson(responsiblePersonId, startValue, countValue));
            var result = MapPagedProjectsToContractList(dbMetadataList);

            return result;
        }

        public PagedResultList<RoleContract> GetRolesByProject(long projectId, int? start, int? count, string query)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            //TODO count, start, search
            var groups = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupsByBook(projectId));
            var groupsCount = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupsByBookCount(projectId));
            
            if (groups == null)
            {
                return null;
            }

            var client = m_communicationProvider.GetAuthRoleApiClient();
            var authRoles = new List<AuthRoleContract>();

            foreach (var group in groups)
            {
                var authRole = client.HttpClient.GetItemAsync<AuthRoleContract>(group.ExternalId).GetAwaiter().GetResult();
                authRoles.Add(authRole);
                if (authRole.Name != group.Name)
                {
                    new SynchronizeRoleWork(m_permissionRepository, authRole).Execute();
                }
            }

            var result = Mapper.Map<List<RoleContract>>(authRoles);

            return new PagedResultList<RoleContract>
            {
                List = result,
                TotalCount = groupsCount,
            };
        }
    }
}