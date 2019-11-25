﻿class PermissionApiClient extends WebHubApiClient {
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

    public createRole(createRoleDataForm: string): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "CreateRole",
            createRoleDataForm,
            this.formContentType,
            this.htmlDataType
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

    public editRole(editRoleDataForm: string): JQuery.jqXHR {
        return this.post(
            this.getPermissionControllerUrl() + "EditRole",
            editRoleDataForm,
            this.formContentType,
            this.htmlDataType
        );
    }

    public getRolesByProject(projectId: number): JQuery.jqXHR<string> {
        return this.get(URI(this.getPermissionControllerUrl() + "RolesByProject").search(query => {
            query.projectId = projectId;
        }).toString());
    }

    public removeProjectFromRole(projectId: number, roleId: number): JQuery.jqXHR<string> {
        return this.post(this.getPermissionControllerUrl() + "RemoveProjectsFromRole",
            JSON.stringify({ roleId: roleId, bookId: projectId }));
    }

    getPermissionForRoleAndBook(projectId: number, roleId: number): JQuery.jqXHR<string> {
        return this.get(URI(this.getPermissionControllerUrl() + "GetPermissionsForRoleAndBook").search(query => {
            query.roleId = roleId;
            query.bookId = projectId;
        }).toString());
    }

    updateOrAddProjectToRole(data: IAddProjectToUserGroupRequest): JQuery.jqXHR<string> {
        return this.post(this.getPermissionControllerUrl() + "UpdateOrAddProjectsToRole",
            JSON.stringify(data));
    }

    addProjectToRole(data: IAddProjectToUserGroupRequest): JQuery.jqXHR<string> {
        return this.post(this.getPermissionControllerUrl() + "AddProjectsToRole",
            JSON.stringify(data));
    }

    addProjectToSingleUser(data: IAddProjectToSingleUserGroupRequest): JQuery.jqXHR<string> {
        return this.post(this.getPermissionControllerUrl() + "AddProjectsToSingleUserGroup",
            JSON.stringify(data));
    }

    public resetUserPassword(userId: number): JQuery.jqXHR  {
        return this.post(
            this.getPermissionControllerUrl() + "ResetUserPassword",
            JSON.stringify({ userId: userId })
        );
    }

    private getPermissionControllerUrl(): string {
        return getBaseUrl() + "Permission/";
    }
}