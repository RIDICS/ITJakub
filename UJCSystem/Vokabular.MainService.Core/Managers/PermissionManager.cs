using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly PermissionRepository m_permissionRepository;

        public PermissionManager(AuthenticationManager authenticationManager, AuthorizationManager authorizationManager, PermissionRepository permissionRepository)
        {
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
            m_permissionRepository = permissionRepository;
        }
        
        public List<SpecialPermissionContract> GetSpecialPermissionsForUser(SpecialPermissionCategorizationEnumContract? filterByType)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var categorizationType = Mapper.Map<SpecialPermissionCategorization?>(filterByType);

            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x =>
                categorizationType == null
                    ? x.GetSpecialPermissionsByUser(userId)
                    : x.GetSpecialPermissionsByUserAndType(userId, categorizationType.Value));
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissions());
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public List<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByGroup(groupId));
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            new AddSpecialPermissionsToGroupWork(m_permissionRepository, groupId, specialPermissionsIds).Execute();
        }

        public void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            new RemoveSpecialPermissionsFromGroupWork(m_permissionRepository, groupId, specialPermissionsIds).Execute();
        }

        public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new AddProjectsToUserGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }

        public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new RemoveProjectsFromUserGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }
    }
}
