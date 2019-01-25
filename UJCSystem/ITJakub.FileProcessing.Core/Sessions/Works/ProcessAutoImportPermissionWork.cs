using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Communication;
using Vokabular.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Const;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class ProcessAutoImportPermissionWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly long m_projectId;
        private readonly List<BookTypeEnum> m_bookTypes;
        private readonly FileProcessingCommunicationProvider m_communicationProvider;

        public ProcessAutoImportPermissionWork(PermissionRepository permissionRepository, long projectId,
            List<BookTypeEnum> bookTypes, FileProcessingCommunicationProvider communicationProvider) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_projectId = projectId;
            m_bookTypes = bookTypes;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var roles = new List<RoleContract>();
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetAllPermissions();
                foreach (var bookType in m_bookTypes)
                {
                    foreach (var permission in permissions.Where(x => x.Name == PermissionNames.AutoImport + (int)bookType).ToList())
                    {
                        roles.AddRange(permission.Roles);
                    }
                }
            }

            var project = m_permissionRepository.Load<Project>(m_projectId);

            var groups = new List<UserGroup>();
            foreach (var role in roles)
            {
                var permission = m_permissionRepository.FindGroupByExternalId(role.Id);
                if(permission != null)
                    groups.Add(permission);
            }
            
            var newPermissions = groups.Select(group => new Permission
            {
                Project = project,
                UserGroup = group
            });

            foreach (var newPermission in newPermissions)
            {
                m_permissionRepository.CreatePermissionIfNotExist(newPermission);
            }
        }
    }
}