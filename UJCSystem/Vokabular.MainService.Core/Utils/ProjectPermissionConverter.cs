using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Utils
{
    public class ProjectPermissionConverter
    {
        private readonly IMapper m_mapper;

        public ProjectPermissionConverter(IMapper mapper)
        {
            m_mapper = mapper;
        }

        public PermissionFlag GetFlags(PermissionDataContract data)
        {
            var flags = PermissionFlag.None;

            if (data.ShowPublished) flags |= PermissionFlag.ShowPublished;
            if (data.ReadProject) flags |= PermissionFlag.ReadProject;
            if (data.EditProject) flags |= PermissionFlag.EditProject;
            if (data.AdminProject) flags |= PermissionFlag.AdminProject;

            return flags;
        }

        // Conversion in opposite way is done by AutoMapper

        public PermissionDataContract GetAggregatedPermissions(IEnumerable<Permission> permissions)
        {
            var joinedFlags = PermissionFlag.None;
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    joinedFlags |= permission.Flags;
                }
            }

            var resultPermissionContract = m_mapper.Map<PermissionDataContract>(joinedFlags);
            return resultPermissionContract;
        }
    }
}