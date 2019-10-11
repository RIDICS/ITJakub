using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
        private readonly IMapper m_mapper;

        public ProjectManager(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            PermissionRepository permissionRepository, AuthenticationManager authenticationManager,
            UserDetailManager userDetailManager, CommunicationProvider communicationProvider, IMapper mapper)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_permissionRepository = permissionRepository;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_communicationProvider = communicationProvider;
            m_mapper = mapper;
        }

        public long CreateProject(ProjectContract projectData)
        {
            var currentUserId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateProjectWork(m_projectRepository, projectData, currentUserId, m_mapper);

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

        public PagedResultList<ProjectDetailContract> GetProjectList(int? start, int? count, ProjectTypeContract? projectType,
            string filterByName, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCountForProject(count);
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum?>(projectType);

            var work = new GetProjectListWork(m_projectRepository, m_metadataRepository, startValue, countValue, projectTypeEnum, filterByName, fetchPageCount, fetchAuthors, fetchResponsiblePersons);
            var resultEntities = work.Execute();

            var metadataList = work.GetMetadataResources();
            var pageCountList = work.GetPageCountList();
            var resultList = m_mapper.Map<List<ProjectDetailContract>>(resultEntities);
            foreach (var projectContract in resultList)
            {
                var metadataResource = metadataList.FirstOrDefault(x => x.Resource.Project.Id == projectContract.Id);
                var pageCountResult = pageCountList.FirstOrDefault(x => x.ProjectId == projectContract.Id);

                var metadataContract = m_mapper.Map<ProjectMetadataContract>(metadataResource);
                projectContract.LatestMetadata = metadataContract;
                projectContract.PageCount = pageCountResult?.PageCount;
                projectContract.CreatedByUser = m_userDetailManager.GetUserContractForUser(projectContract.CreatedByUser);

                if (fetchAuthors && metadataResource != null)
                    projectContract.Authors = m_mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

                if (fetchResponsiblePersons && metadataResource != null)
                    projectContract.ResponsiblePersons =
                        m_mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);
            }

            return new PagedResultList<ProjectDetailContract>
            {
                List = resultList,
                TotalCount = work.GetResultCount()
            };
        }

        public ProjectDetailContract GetProject(long projectId, bool fetchPageCount, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            var work = new GetProjectWork(m_projectRepository, m_metadataRepository, projectId, fetchPageCount, fetchAuthors, fetchResponsiblePersons, false);
            var project = work.Execute();

            if (project == null)
            {
                return null;
            }

            var metadataResource = work.GetMetadataResource();
            var result = m_mapper.Map<ProjectDetailContract>(project);
            result.CreatedByUser = m_userDetailManager.GetUserContractForUser(result.CreatedByUser);
            result.LatestMetadata = m_mapper.Map<ProjectMetadataContract>(metadataResource);
            result.PageCount = work.GetPageCount();

            if (fetchAuthors && metadataResource != null)
                result.Authors = m_mapper.Map<List<OriginalAuthorContract>>(metadataResource.Resource.Project.Authors);

            if (fetchResponsiblePersons && metadataResource != null)
                result.ResponsiblePersons =
                    m_mapper.Map<List<ProjectResponsiblePersonContract>>(metadataResource.Resource.Project.ResponsiblePersons);

            return result;
        }

        private List<ProjectDetailContract> MapProjectsToContractList(IEnumerable<MetadataResource> dbMetadataList)
        {
            var resultList = new List<ProjectDetailContract>();
            foreach (var metadataResource in dbMetadataList)
            {
                var project = metadataResource.Resource.Project;
                var resultItem = m_mapper.Map<ProjectDetailContract>(project);
                resultItem.CreatedByUser = m_userDetailManager.GetUserContractForUser(resultItem.CreatedByUser);
                resultItem.LatestMetadata = m_mapper.Map<ProjectMetadataContract>(metadataResource);
                resultItem.Authors = m_mapper.Map<List<OriginalAuthorContract>>(project.Authors);
                resultItem.ResponsiblePersons = m_mapper.Map<List<ProjectResponsiblePersonContract>>(project.ResponsiblePersons);

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

        public PagedResultList<RoleContract> GetRolesByProject(long projectId, int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupsByBook(projectId, startValue, countValue, filterByName));

            if (result == null)
            {
                return null;
            }

            var authRoles = new List<AuthRoleContract>();
            foreach (var group in result.List)
            {
                var work = new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, group.ExternalId);
                work.Execute();
                authRoles.Add(work.GetRoleContract());
            }

            var roles = m_mapper.Map<List<RoleContract>>(authRoles);

            return new PagedResultList<RoleContract>
            {
                List = roles,
                TotalCount = result.Count,
            };
        }
    }
}