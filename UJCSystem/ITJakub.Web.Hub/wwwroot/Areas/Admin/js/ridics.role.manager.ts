$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();
});

class RoleManager {
    private readonly searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private currentUserSelectedItem: IUserDetail;
    private readonly client: WebHubApiClient;
    private userList: ListWithPagination;
    private roleList: ListWithPagination;

    constructor() {
        this.searchBox = new SingleSetTypeaheadSearchBox<IUserDetail>("#mainSearchInput",
            "Permission",
            this.getFullNameString,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(this.getFullNameString(item),
                item.email));
        this.client = new WebHubApiClient();
    }

    public init(list?: ListWithPagination) {
        if (list == null) {
            this.roleList = new ListWithPagination("Permission/RolePermission", 10, "role", ViewType.Widget, true, undefined, this);
            
        } else {
            this.roleList = list;
        }
        this.roleList.init();

        $(".role-row").click((event) => {
            $(event.currentTarget).addClass("active").siblings().removeClass("active");

            var selectedRoleId = $(event.currentTarget).data("role-id");
            this.loadUsers(selectedRoleId);
            this.loadPermissions(selectedRoleId);
        });

        this.initCreateRoleModal();
        this.initRemoveRoleButtons();
        this.initSearchBox();
    }

    private loadUsers(roleId: number) {
        const userSection = $("#user-section .section");
        const container = userSection.find("#user-list-container");
        container.html("<div class=\"loader\"></div>");
        userSection.removeClass("hide");

        this.client.getUsersByRole(roleId).done(response => {
            container.html(response);
            this.initRemoveUserFromRoleButton();
        }).fail(() => {
            container.html(`<div class="alert alert-danger">${localization.translate("ListError", "PermissionJs").value}</div>`);
        }).always(() => {
            this.userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`,
                10,
                "user",
                ViewType.Widget,
                false,
                this.initRemoveUserFromRoleButton,
                this);
            this.userList.init();
        });
    }

    private loadPermissions(roleId: number) {
        const permissionSection = $("#permission-section .section");
        const container = permissionSection.find("#permission-list-container");
        container.html("<div class=\"loader\"></div>");
        permissionSection.removeClass("hide");

        this.client.getPermissionsByRole(roleId).done(response => {
            container.html(response);
            this.initPermissionManaging();
        }).fail(() => {
            container.html(`<div class="alert alert-danger">${localization.translate("ListError", "PermissionJs").value}</div>`);
        }).always(() => {
            const permissionList = new ListWithPagination(`Permission/RolePermissionList?roleId=${roleId}`,
                10,
                "permission",
                ViewType.Widget,
                false,
                this.initPermissionManaging,
                this);
            permissionList.init();
        });
    }

    private initPermissionManaging() {

        $(".permission-row input[type=checkbox]").on("click", (event) => {
            const permissionCheckboxInput = $(event.currentTarget);
            const addPermission = $(event.currentTarget).is(":checked");
            const permissionRow = permissionCheckboxInput.parents(".permission-row");
            permissionRow.addClass("pending");
            const alert = permissionRow.find(".alert");
            alert.hide();
            const specialPermissionId = permissionRow.data("permission-id");
            const roleId = $(".role-row.active").data("role-id");

            let result: JQuery.jqXHR<any>;
            if (addPermission) {
                result = this.client.addSpecialPermissionToRole(roleId, specialPermissionId);
            } else {
                result = this.client.removeSpecialPermissionToRole(roleId, specialPermissionId);
            }

            result.done(() => {
            }).fail(() => {
                permissionCheckboxInput.prop("checked", !addPermission);
                alert.text(localization.translate("PermissionError", "PermissionJs").value);
                alert.show();
            }).always(() => {
                permissionRow.removeClass("pending");
            });
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-user-from-role").click((event) => {
            var userRow = $(event.currentTarget).parents(".user-row");
            var userId = userRow.data("user-id");
            const alert = userRow.find(".alert");
            alert.hide();

            var roleId = $(".role-row.active").data("role-id");
            this.client.removeUserFromRole(userId, roleId).done(() => {
                this.userList.reloadPage();
            }).fail(() => {
                alert.text(localization.translate("RemoveUserFromRoleError", "PermissionJs").value);
                alert.show();
            });
        });
    }

    private initRemoveRoleButtons() {
        $(".remove-role").click((event) => {
            event.stopPropagation();
            var roleRow = $(event.currentTarget).parents(".role-row");
            var roleId = roleRow.data("role-id");
            var alert = roleRow.find(".alert");
            alert.hide();
            this.client.deleteRole(roleId).done(() => {
                this.roleList.reloadPage();
            }).fail(() => {
                alert.text(localization.translate("RemoveRoleError", "PermissionJs").value);
                alert.show();
            });
        });
    }

    private initSearchBox() {
        this.searchBox.setDataSet("User");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (!selectedExists || this.searchBox.getInputValue() === "") {

            }

            if (selectionConfirmed) {
                this.currentUserSelectedItem = this.searchBox.getValue();
                var userBox = $("#selectedUser");
                var name = this.getFullNameString(this.currentUserSelectedItem);
                userBox.text(name);
                userBox.data("user-id", this.currentUserSelectedItem.id);
            }
        });

        const addUserToRoleBtn = $("#add-user-to-role");
        if (addUserToRoleBtn.data("init") === false) {
            addUserToRoleBtn.data("init", true);

            $("#addRoleButton").click(() => {
                var role = $(".role-row.active");
                $("#specificRoleName").text(role.find(".name").text());
                $("#specificRoleDescription").text(role.find(".description").text());
                $("#addToRoleDialog").modal();
            });

            addUserToRoleBtn.click(() => {
                var roleError = $("#add-user-to-role-error");
                roleError.html("");
                var roleId = $(".role-row.active").data("role-id");
                var userId: number;
                if (typeof this.currentUserSelectedItem == "undefined")
                    userId = $("#selectedUser").data("user-id");
                else {
                    userId = this.currentUserSelectedItem.id;
                }

                this.client.addUserToRole(userId, roleId).done(() => {
                    this.userList.reloadPage();
                    $("#addToRoleDialog").modal("hide");
                }).fail(() => {
                    roleError.html(`<div class="alert alert-danger">${localization.translate("AddUserToRoleError", "PermissionJs").value}</div>`);
                });
            });
        }
    }

    private getFullNameString(user: IUser): string {
        return `${user.userName} - ${user.firstName} - ${user.lastName}`;
    }

    private initCreateRoleModal() {
        $("#createRoleButton").click(() => {
            $("#createRoleModal").modal();
        });

        $("#create-role").click(() => {
            var roleName = $("#new-role-name").val() as string;
            var roleDescription = $("#new-role-description").val() as string;
            var roleError = $(".add-user-to-role-error");
            roleError.html("");
            this.client.createRole(roleName, roleDescription).done(() => {
                $("#createRoleModal").modal("hide");
                this.roleList.reloadPage();
            }).fail(() => {
                roleError.html(`<div class="alert alert-danger">${localization.translate("CreateRoleError", "PermissionJs").value}</div>`);
            });
        });
    }
}