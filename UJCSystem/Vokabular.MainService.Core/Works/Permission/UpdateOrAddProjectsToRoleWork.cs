﻿using System.Collections.Generic;
using System.Data;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class UpdateOrAddProjectsToRoleWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_roleId;
        private readonly IList<long> m_bookIds;
        private readonly PermissionFlag m_permissionFlags;
        private readonly ProjectPermissionsSubwork m_permissionsSubwork;

        public UpdateOrAddProjectsToRoleWork(PermissionRepository permissionRepository, int roleId, IList<long> bookIds,
            PermissionFlag permissionFlags) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_roleId = roleId;
            m_bookIds = bookIds;
            m_permissionFlags = permissionFlags;
            m_permissionsSubwork = new ProjectPermissionsSubwork(m_permissionRepository);
        }

        protected override void ExecuteWorkImplementation()
        {
            m_permissionsSubwork.CheckPermissionConsistency(m_permissionFlags);

            var group = m_permissionRepository.FindById<UserGroup>(m_roleId);

            var allBookIds = new List<long>();

            if (m_bookIds != null)
            {
                allBookIds.AddRange(m_bookIds);
            }

            var permissionsList = new List<DataEntities.Database.Entities.Permission>();

            foreach (var bookId in allBookIds)
            {
                var dbPermission = m_permissionRepository.FindPermissionByBookAndGroup(bookId, group.Id);
                if (dbPermission != null) // Permission already exists
                {
                    dbPermission.Flags = m_permissionFlags;
                    m_permissionRepository.Update(dbPermission);
                }
                else
                {
                    var book = m_permissionRepository.Load<Project>(bookId);
                    permissionsList.Add(new DataEntities.Database.Entities.Permission
                    {
                        Project = book,
                        UserGroup = group,
                        Flags = m_permissionFlags,
                    });
                }
            }

            foreach (var permission in permissionsList)
            {
                try
                {
                    m_permissionRepository.Create(permission);
                }
                catch (DataException ex)
                {
                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Cannot save permission for group witd id '{0}' on book with id '{1}' for reason '{2}'", permission.UserGroup.Id, permission.Project.Id, ex.InnerException);
                    throw;
                }
            }

            foreach (var bookId in allBookIds)
            {
                m_permissionsSubwork.CheckRemainingAdministrator(bookId);
            }
        }
    }
}