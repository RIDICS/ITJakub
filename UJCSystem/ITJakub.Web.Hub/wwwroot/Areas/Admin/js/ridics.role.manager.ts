$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();

    var groupList = new ListWithPagination("Permission/GroupPermission", 10, "role", ViewType.Widget, true, roleManager.init);
    groupList.init();
});

class RoleManager {
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
                var userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`,
                    10,
                    "user",
                    ViewType.Widget,
                    false);
                userList.init();
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
                groupId: $(".role-row.active").data("role-id"),
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
}