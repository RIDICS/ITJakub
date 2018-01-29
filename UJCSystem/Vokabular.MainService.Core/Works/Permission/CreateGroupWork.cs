using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class CreateGroupWork : UnitOfWorkBase<int>
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly string m_groupName;
        private readonly string m_description;
        private readonly int m_userId;

        public CreateGroupWork(PermissionRepository permissionRepository, string groupName, string description, int userId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupName = groupName;
            m_description = description;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_permissionRepository.Load<User>(m_userId);

            var group = new UserGroup
            {
                Name = m_groupName,
                Description = m_description,
                CreateTime = now,
                CreatedBy = user
            };

            var groupId = m_permissionRepository.CreateGroup(group);
            return groupId;
        }
    }
}