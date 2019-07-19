﻿$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();
});

class RoleManager {
    private searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private currentUserSelectedItem: IUserDetail;
    public client;
    public userList;
    public roleList: ListWithPagination;

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

        this.client.getUsersByRole(roleId).then(response => {
            if (response.hasOwnProperty("message")) {
                container.html(`<div class="alert alert-danger">${response.message}</div>`);
            } else {
                container.html(response);
                this.initRemoveUserFromRoleButton();
            }
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

        this.client.getPermissionsByRole(roleId).then(response => {
            if (response.hasOwnProperty("message")) {
                container.html(`<div class="alert alert-danger">${response.message}</div>`);
            } else {
                container.html(response);
                this.initPermissionManaging();
            }
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
        $(".permission-row input[type=checkbox]").change((event) => {
            event.preventDefault();
            const permissionCheckboxInput = $(event.currentTarget);
            const permissionRow = permissionCheckboxInput.parents(".permission-row");
            permissionRow.addClass("pending");
            const alert = permissionRow.find(".alert");
            alert.hide();
            const specialPermissionId = permissionRow.data("permission-id");
            const roleId = $(".role-row.active").data("role-id");

            if ($(event.currentTarget).is(":checked")) {
                this.client.addSpecialPermissionToRole(roleId, specialPermissionId).then((response) => {
                    if (response.hasOwnProperty("message")) {
                        alert.text(response.message);
                        alert.show();
                    } else {
                        permissionCheckboxInput.prop("checked", !permissionCheckboxInput.prop("checked"));
                    }
                    permissionRow.removeClass("pending");
                });
            } else {
                this.client.removeSpecialPermissionToRole(roleId, specialPermissionId).then((response) => {
                    if (response.hasOwnProperty("message")) {
                        alert.text(response.message);
                        alert.show();
                    } else {
                        permissionCheckboxInput.prop("checked", !permissionCheckboxInput.prop("checked"));
                    }
                    permissionRow.removeClass("pending");
                });
            }
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-user-from-role").click((event) => {
            var userId = $(event.currentTarget).data("user-id");
            var roleId = $(".role-row.active").data("role-id");
            this.client.removeUserFromRole(userId, roleId).then(() => {
                this.userList.reloadPage();
            });
        });
    }

    private initRemoveRoleButtons() {
        $(".remove-role").click((event) => {
            event.stopPropagation();
            var roleId = $(event.currentTarget).parents("tr.role-row").data("role-id");
            this.client.deleteRole(roleId).then(() => {
                this.roleList.reloadPage();
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
                var roleId = $(".role-row.active").data("role-id");
                var userId: number;
                if (typeof this.currentUserSelectedItem == "undefined")
                    userId = $("#selectedUser").data("user-id");
                else {
                    userId = this.currentUserSelectedItem.id;
                }

                this.client.addUserToRole(userId, roleId).then(() => {
                    this.userList.reloadPage();
                    $("#addToRoleDialog").modal("hide");
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
            this.client.createRole(roleName, roleDescription).then(() => {
                $("#createRoleModal").modal("hide");
                this.roleList.reloadPage();
            });
        });
    }
}