class PermissionApiClient extends WebHubApiClient {
    public getUsersByRole(roleId: number): JQuery.jqXHR {
        return this.get(URI(this.getPermissionControllerUrl() + "UsersByRole").search(query => {
            query.roleId = roleId;
        }).toString());
    }

    public getPermissionsByRole(roleId: number): JQuery.jqXHR {
        return this.get(URI(this.getPermissionControllerUrl() + "RolePermissionList").search(query => {
            query.roleId = roleId;
        }).toString());
    }

    public addSpecialPermissionToRole(roleId: number, permissionId: number): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "AddSpecialPermissionsToRole",
            JSON.stringify({ roleId: roleId, specialPermissionId: permissionId })
        );
    }

    public removeSpecialPermissionToRole(roleId: number, permissionId: number): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "RemoveSpecialPermissionsFromRole",
            JSON.stringify({ roleId: roleId, specialPermissionId: permissionId })
        );
    }

    public createRole(roleName: string, roleDescription: string): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "CreateRole",
            JSON.stringify({ roleName: roleName, roleDescription: roleDescription })
        );
    }

    public createRoleWithUser(userId: number, roleName: string, roleDescription: string): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "CreateRoleWithUser",
            JSON.stringify({ userId: userId, roleName: roleName, roleDescription: roleDescription })
        );
    }

    public removeUserFromRole(userId: number, roleId: number): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "RemoveUserFromRole",
            JSON.stringify({ userId: userId, roleId: roleId })
        );
    }

    public addUserToRole(userId: number, roleId: number): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "AddUserToRole",
            JSON.stringify({ userId: userId, roleId: roleId })
        );
    }

    public deleteRole(roleId: number): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "DeleteRole",
            JSON.stringify({ roleId: roleId })
        );
    }

    private getPermissionControllerUrl(): string {
        return getBaseUrl() + "Permission/";
    }
}