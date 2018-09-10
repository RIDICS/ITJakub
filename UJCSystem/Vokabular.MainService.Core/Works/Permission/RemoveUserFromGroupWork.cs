using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveUserFromGroupWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_userId;
        private readonly int m_groupId;

        public RemoveUserFromGroupWork(PermissionRepository permissionRepository, int userId, int groupId) : base(permissionRepository)
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
                if (m_log.IsWarnEnabled)
                {
                    string message = string.Format("Cannot remove user with id '{0}' from group with id '{1}'. Group is empty.", user.Id, group.Id);
                    m_log.Warn(message);
                }
                return;
            }

            group.Users.Remove(user);
            m_permissionRepository.Save(group);
        }
    }
}