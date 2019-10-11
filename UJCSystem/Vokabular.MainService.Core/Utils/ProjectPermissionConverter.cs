using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Utils
{
    public class ProjectPermissionConverter
    {
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
    }
}