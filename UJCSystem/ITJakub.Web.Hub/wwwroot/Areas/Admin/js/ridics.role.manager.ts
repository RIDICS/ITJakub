$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();

    var groupList = new ListWithPagination("Permission/GroupPermission", 10, "role", ViewType.Widget, true, roleManager.init);
    groupList.init();
});

class RoleManager {
    private userList;
    public init() {
        $(".role-row").click((event) => {
            $(event.currentTarget).addClass("active").siblings().removeClass("active");

            var selectedRoleId = $(event.currentTarget).data("role-id");
            this.loadUsers(selectedRoleId);
            this.loadPermissions(selectedRoleId);
        });
    }

    private loadUsers(roleId: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: URI(getBaseUrl() + "Permission/UsersByRole").search(query => {
                query.roleId = roleId;
            }).toString(),
            success: (response) => {
                $("#user-list-container").html(response);
                this.userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`,
                    10,
                    "user",
                    ViewType.Widget,
                    false,
                    this.initRemoveUserFromGroupButton);
                this.userList.init();
                this.initRemoveUserFromGroupButton();
                $("#user-section .section").removeClass("hide");
            },
        });
    }

    private loadPermissions(roleId: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: URI(getBaseUrl() + "Permission/GroupPermissionList").search(query => {
                query.roleId = roleId;
            }).toString(),
            success: (response) => {
                $("#permission-list-container").html(response);
                var permissionList = new ListWithPagination(`Permission/GroupPermissionList?roleId=${roleId}`,
                    10,
                    "permission",
                    ViewType.Widget,
                    false,
                    this.initPermissionManaging);
                permissionList.init();
                this.initPermissionManaging();
                $("#permission-section .section").removeClass("hide");
            },
        });
    }

    private initPermissionManaging() {
        $(".permission-checkbox input[type=checkbox]").change((event) => {
            var data = JSON.stringify({
                groupId: this.getSelectedRoleId(),
                specialPermissionId: $(event.currentTarget).parent("td").data("permission-id")
            });

            let urlPath: string;
            if ($(event.currentTarget).is(":checked")) {
                urlPath = "Permission/AddSpecialPermissionsToGroup";
            } else {
                urlPath = "Permission/RemoveSpecialPermissionsFromGroup";
            }

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + urlPath,
                data: data,
                dataType: "json",
                contentType: "application/json"
            });
        });
    }

    private getSelectedRoleId(): number {
        return $(".role-row.active").data("role-id");
    }

    private initRemoveUserFromGroupButton() {
        $(".remove-user-from-group").click((event) => {
            var userId = $(event.currentTarget).data("user-id");
            var roleId = this.getSelectedRoleId();
            this.removeUserFromRole(userId, roleId);
        });
    }

    private removeUserFromRole(userId: number, roleId: number) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Permission/RemoveUserFromGroup",
            data: JSON.stringify({ userId: userId, groupId: roleId }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                console.log(this.userList);
                this.userList.reloadPage();
            },
            error: (response) => {
                console.log(this.userList);
                this.userList.reloadPage();
            }
        });
    }
}