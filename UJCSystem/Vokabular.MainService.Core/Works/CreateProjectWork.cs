using System;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;
using PermissionEntity = Vokabular.DataEntities.Database.Entities.Permission;

namespace Vokabular.MainService.Core.Works
{
    public class CreateProjectWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectContract m_newData;
        private readonly int m_userId;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly IMapper m_mapper;

        public CreateProjectWork(ProjectRepository projectRepository, ProjectContract newData, int userId, DefaultUserProvider defaultUserProvider, IMapper mapper) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_newData = newData;
            m_userId = userId;
            m_defaultUserProvider = defaultUserProvider;
            m_mapper = mapper;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var currentUser = m_projectRepository.Load<User>(m_userId);
            var projectType = m_mapper.Map<ProjectTypeEnum>(m_newData.ProjectType);

            // Create project
            var project = new Project
            {
                Name = m_newData.Name,
                ProjectType = projectType,
                TextType = TextTypeEnum.Transcribed, // TODO get from parameter
                CreateTime = now,
                CreatedByUser = currentUser
            };

            var projectId = (long) m_projectRepository.Create(project);

            // Set default permissions
            var unregisteredUserGroup = m_defaultUserProvider.GetDefaultUnregisteredUserGroup();
            var registeredUsersGroup = m_defaultUserProvider.GetDefaultRegisteredUserGroup();
            var singleUserGroup = currentUser.Groups.OfType<SingleUserGroup>().SingleOrDefault();
            var permission1 = new PermissionEntity
            {
                Project = project,
                UserGroup = unregisteredUserGroup,
                Flags = PermissionFlag.ShowPublished,
            };
            var permission2 = new PermissionEntity
            {
                Project = project,
                UserGroup = registeredUsersGroup,
                Flags = PermissionFlag.ShowPublished,
            };
            var permission3 = new PermissionEntity
            {
                Project = project,
                UserGroup = singleUserGroup,
                Flags = PermissionFlag.All,
            };

            m_projectRepository.Create(permission1);
            m_projectRepository.Create(permission2);
            m_projectRepository.Create(permission3);

            return projectId;
        }
    }
}
