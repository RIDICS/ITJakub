using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class ProcessAutoImportPermissionWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly long m_projectId;
        private readonly List<BookTypeEnum> m_bookTypes;

        public ProcessAutoImportPermissionWork(PermissionRepository permissionRepository, long projectId,
            List<BookTypeEnum> bookTypes) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_projectId = projectId;
            m_bookTypes = bookTypes;
        }

        protected override void ExecuteWorkImplementation()
        {
            var specialPermissions = m_permissionRepository.GetAutoimportPermissionsByBookTypeList(m_bookTypes);
            var groupsWithAutoimport = m_permissionRepository.GetGroupsBySpecialPermissionIds(specialPermissions.Select(x => x.Id));
            var project = m_permissionRepository.Load<Project>(m_projectId);

            var newPermissions = groupsWithAutoimport.Select(groupWithAutoimport => new Permission
            {
                Project = project,
                UserGroup = groupWithAutoimport
            });

            foreach (var newPermission in newPermissions)
            {
                m_permissionRepository.CreatePermissionIfNotExist(newPermission);
            }
        }
    }
}