using Ridics.Authentication.DataContracts;

namespace Vokabular.ProjectImport.Permissions
{
    public interface IPermissionsProvider
    {
        PermissionContract GetPermissionByName(string name);
    }
}
