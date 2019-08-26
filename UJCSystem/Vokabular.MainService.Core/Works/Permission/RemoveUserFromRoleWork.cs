using System.Net;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts;

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
            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_roleId);
            var user = m_permissionRepository.GetUserWithGroups(m_userId);
            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    new object[] {user.Id}
                );
            }

            if (user.Groups == null)
            {
                if (m_log.IsWarnEnabled)
                {
                    string message = $"Cannot remove Group with ID '{group.Id}' from User with ID '{user.Id}'. User is empty.";
                    m_log.Warn(message);
                }

                return;
            }

            // Remove group from user (fetch lower amount of data)

            user.Groups.Remove(group);
            m_permissionRepository.Save(user);
            m_permissionRepository.Flush();


            var client = m_communicationProvider.GetAuthUserApiClient();
            client.RemoveRoleFromUserAsync(user.ExternalId.Value, m_roleId).GetAwaiter().GetResult();
        }
    }
}