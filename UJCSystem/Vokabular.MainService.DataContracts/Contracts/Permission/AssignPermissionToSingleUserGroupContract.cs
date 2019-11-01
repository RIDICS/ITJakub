namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class AssignPermissionToSingleUserGroupContract
    {
        public string Code { get; set; }

        public PermissionDataContract Permissions { get; set; }
    }
}