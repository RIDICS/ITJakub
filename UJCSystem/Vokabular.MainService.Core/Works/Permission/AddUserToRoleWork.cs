﻿using System.Collections.Generic;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class AddUserToRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_userId;
        private readonly int m_roleId;

        public AddUserToRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, int userId,
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
                    new object[] { user.Id }
                );
            }

            if (user.Groups == null)
            {
                user.Groups = new List<UserGroup>();
            }

            // Assign group to user (fetch lower amount of data)

            user.Groups.Add(group);
            m_permissionRepository.Save(user);
            m_permissionRepository.Flush();


            var client = m_communicationProvider.GetAuthUserApiClient();
            client.AddRoleToUserAsync(user.ExternalId.Value, m_roleId).GetAwaiter().GetResult();
        }
    }
}
