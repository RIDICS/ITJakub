using System;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveUserFromRoleWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_userId;
        private readonly int m_roleId;

        public RemoveUserFromRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, int userId,
            int roleId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_userId = userId;
            m_roleId = roleId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupByExternalId(m_roleId);
            var user = m_permissionRepository.FindById<User>(m_userId);
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
            }

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
            m_permissionRepository.Flush();


            var client = m_communicationProvider.GetAuthUserApiClient();
            client.RemoveRoleFromUserAsync(user.ExternalId.Value, m_roleId).GetAwaiter().GetResult();
        }
    }
}
