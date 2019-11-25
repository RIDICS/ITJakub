﻿using System;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class CreateRoleWork : UnitOfWorkBase<int>
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly string m_roleName;
        private readonly string m_description;

        public CreateRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, string roleName, string description) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_roleName = roleName;
            m_description = description;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var roleContract = new RoleContractBase
            {
                Description = m_description,
                Name = m_roleName,
            };

            var response = client.CreateRoleAsync(roleContract).GetAwaiter().GetResult();
            var externalRoleId = response.Content.ReadAsInt();

            var group = new RoleUserGroup
            {
                Name = m_roleName,
                CreateTime = now,
                LastChange = now,
                ExternalId = externalRoleId,
            };

            var roleId = m_permissionRepository.CreateGroup(group);
            return roleId;
        }
    }
}
