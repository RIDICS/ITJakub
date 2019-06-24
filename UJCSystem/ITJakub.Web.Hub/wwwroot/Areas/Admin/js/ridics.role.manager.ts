$(document.documentElement).ready(() => {
    var roleList = new ListWithPagination("Permission/RolePermission", 10, "role", ViewType.Widget, true);
    roleList.init();

    var roleManager = new RoleManager();
    roleManager.init(roleList);
});

class RoleManager {
    public static roleSectionSelector = "#role-section";

    private userList;
    public init(roleList: ListWithPagination) {
        $(".role-row").click((event) => {
            $(event.currentTarget).addClass("active").siblings().removeClass("active");

            var selectedRoleId = $(event.currentTarget).data("role-id");
            this.loadUsers(selectedRoleId);
            this.loadPermissions(selectedRoleId);
        });

        this.initRemoveRoleButtons(roleList);
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
                    this.initRemoveUserFromRoleButton);
                this.userList.init();
                this.initRemoveUserFromRoleButton(this.userList);
                $("#user-section .section").removeClass("hide");
            },
        });
    }

    private loadPermissions(roleId: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: URI(getBaseUrl() + "Permission/RolePermissionList").search(query => {
                query.roleId = roleId;
            }).toString(),
            success: (response) => {
                $("#permission-list-container").html(response);
                var permissionList = new ListWithPagination(`Permission/RolePermissionList?roleId=${roleId}`,
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
                roleId: $(".role-row.active").data("role-id"),
                specialPermissionId: $(event.currentTarget).parent("td").data("permission-id")
            });

            let urlPath: string;
            if ($(event.currentTarget).is(":checked")) {
                urlPath = "Permission/AddSpecialPermissionsToRole";
            } else {
                urlPath = "Permission/RemoveSpecialPermissionsFromRole";
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

    private initRemoveUserFromRoleButton(list: ListWithPagination) {
        $(".remove-user-from-role").click((event) => {
            var userId = $(event.currentTarget).data("user-id");
            var roleId = $(".role-row.active").data("role-id");
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/RemoveUserFromRole",
                data: JSON.stringify({ userId: userId, roleId: roleId }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    list.reloadPage();
                }
            });
        });
    }

    private initRemoveRoleButtons(list: ListWithPagination) {
        $(".remove-role").click((event) => {
            event.stopPropagation();
            var roleId = $(event.currentTarget).parents("tr.role-row").data("role-id");
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/DeleteRole",
                data: JSON.stringify({ roleId: roleId }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    list.reloadPage();
                }
            });
        });
    }
}
