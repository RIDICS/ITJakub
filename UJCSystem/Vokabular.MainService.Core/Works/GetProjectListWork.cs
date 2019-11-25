using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectListWork : UnitOfWorkBase<IList<Project>>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly int m_start;
        private readonly int m_count;
        private readonly ProjectTypeEnum? m_projectType;
        private readonly ProjectOwnerTypeContract m_projectOwnerType;
        private readonly int m_userId;
        private readonly string m_filterByName;
        private readonly bool m_fetchPageCount;
        private readonly bool m_fetchAuthors;
        private readonly bool m_fetchResponsiblePersons;
        private readonly bool m_fetchLatestChangedResource;
        private readonly bool m_fetchPermissions;
        private int m_resultCount;
        private IList<MetadataResource> m_metadataList;
        private IList<PageCountResult> m_pageCount;
        private IList<LatestChangedResourceResult> m_latestChangedResources;
        private Dictionary<long, List<DataEntities.Database.Entities.Permission>> m_permissions;

        public GetProjectListWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, int start, int count,
            ProjectTypeEnum? projectType, ProjectOwnerTypeContract projectOwnerType, int userId, string filterByName, bool fetchPageCount,
            bool fetchAuthors, bool fetchResponsiblePersons, bool fetchLatestChangedResource, bool fetchPermissions) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_start = start;
            m_count = count;
            m_projectType = projectType;
            m_projectOwnerType = projectOwnerType;
            m_userId = userId;
            m_filterByName = filterByName;
            m_fetchPageCount = fetchPageCount;
            m_fetchAuthors = fetchAuthors;
            m_fetchResponsiblePersons = fetchResponsiblePersons;
            m_fetchLatestChangedResource = fetchLatestChangedResource;
            m_fetchPermissions = fetchPermissions;
        }

        protected override IList<Project> ExecuteWorkImplementation()
        {
            int? includeUserId = null;
            int? excludeUserId = null;
            switch (m_projectOwnerType)
            {
                case ProjectOwnerTypeContract.AllProjects:
                    // No filter
                    break;
                case ProjectOwnerTypeContract.MyProjects:
                    includeUserId = m_userId;
                    break;
                case ProjectOwnerTypeContract.ForeignProjects:
                    excludeUserId = m_userId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var dbResult = m_projectRepository.GetProjectList(m_start, m_count, m_projectType, m_filterByName, includeUserId, excludeUserId);
            var projectIdList = dbResult.List.Select(x => x.Id).ToList();

            m_metadataList = m_metadataRepository.GetMetadataByProjectIds(projectIdList, m_fetchAuthors, m_fetchResponsiblePersons, false);
            m_resultCount = dbResult.Count;

            m_pageCount = m_fetchPageCount
                ? m_projectRepository.GetAllPageCount(projectIdList)
                : new List<PageCountResult>();

            m_latestChangedResources = m_fetchLatestChangedResource
                ? m_projectRepository.GetAllLatestChangedResource(projectIdList)
                : new List<LatestChangedResourceResult>();

            if (m_fetchPermissions)
            {
                var permissions = m_projectRepository.FindPermissionsForProjectsByUserId(projectIdList, m_userId);
                m_permissions = permissions.GroupBy(key => key.Project.Id).ToDictionary(key => key.Key, val => val.ToList());
            }
            else
            {
                m_permissions = new Dictionary<long, List<DataEntities.Database.Entities.Permission>>();
            }

            return dbResult.List;
        }

        public int GetResultCount()
        {
            return m_resultCount;
        }

        public IList<MetadataResource> GetMetadataResources()
        {
            return m_metadataList;
        }

        public IList<PageCountResult> GetPageCountList()
        {
            return m_pageCount;
        }

        public IList<LatestChangedResourceResult> GetLatestChangedResources()
        {
            return m_latestChangedResources;
        }

        public Dictionary<long, List<DataEntities.Database.Entities.Permission>> GetUserPermission()
        {
            return m_permissions;
        }
    }
}