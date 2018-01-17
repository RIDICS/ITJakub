using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class AddUserToGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_userId;
        private readonly int m_groupId;

        public AddUserToGroupWork(PermissionRepository permissionRepository, int userId, int groupId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_userId = userId;
            m_groupId = groupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupById(m_groupId);
            var user = m_permissionRepository.Load<User>(m_userId);

            //TODO switch logic: remove group from user (fetch lower amount of data)

            if (group.Users == null)
            {
                group.Users = new List<User>();
            }

            group.Users.Add(user);
            m_permissionRepository.Save(group);
        }
    }
}