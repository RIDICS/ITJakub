using System.Collections.Generic;

namespace Vokabular.ProjectImport.Permissions
{
    public interface IPermissionsProvider
    {
        IList<int> GetRoleIdsByPermissionName(string name);
    }
}
