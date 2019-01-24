using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly PermissionConverter m_permissionConverter;

        public PermissionManager(PermissionRepository permissionRepository, CommunicationProvider communicationProvider,
            PermissionConverter permissionConverter)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_permissionConverter = permissionConverter;
        }

        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetAllPermissions();
                return m_permissionConverter.Convert(permissions);
            }
        }

        public List<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetRole(groupId).Permissions;
                return m_permissionConverter.Convert(permissions);
            }
        }

        public void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetRole(groupId).Permissions;
                var permissionsId = permissions.Select(x => x.Id).ToList();
                foreach (var permissionToAdd in specialPermissionsIds)
                {
                    if (!permissionsId.Contains(permissionToAdd))
                    {
                        permissionsId.Add(permissionToAdd);
                    }
                }
                client.AssignPermissionsToRole(groupId, permissionsId);
            }
        }

        public void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionsIds)
        {
            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetRole(groupId).Permissions;
                var permissionsId = permissions.Select(x => x.Id).ToList();
                foreach (var permissionToRemove in specialPermissionsIds)
                {
                    permissionsId.Remove(permissionToRemove);
                }
                
                client.AssignPermissionsToRole(groupId, permissionsId);
            }
        }

        public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds)
        {
            new AddProjectsToUserGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }

        public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds)
        {
            new RemoveProjectsFromUserGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }
    }
}