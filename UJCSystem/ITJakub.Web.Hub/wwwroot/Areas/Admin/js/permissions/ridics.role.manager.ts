$(document.documentElement).ready(() => {
    var roleManager = new RoleManager();
    roleManager.init();
});

class RoleManager {
    private readonly searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private readonly client: PermissionApiClient;
    private readonly delayForShowResponse = 1000;
    private readonly errorHandler: ErrorHandler;
    private currentUserSelectedItem: IUserDetail;
    private roleList: ListWithPagination; 
    private userList: ListWithPagination;
    private permissionList: ListWithPagination; 

    constructor() {
        this.searchBox = new SingleSetTypeaheadSearchBox<IUserDetail>("#mainSearchInput",
            "Permission",
            this.getFullNameString,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(this.getFullNameString(item),
                item.email));
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
    }

    public init(list?: ListWithPagination) {
        if (list == null) {
            this.roleList = new ListWithPagination("Permission/RolePermission", "role", ViewType.Widget, true, this.reinitRoleList, this);
            
        } else {
            this.roleList = list;
        }
        this.roleList.init();

        this.initCreateRoleModal();
        this.initSearchBox();
        this.reinitRoleList();
        this.initEditRoleForm();
    }

    public reinitRoleList() {
        $(".role-row").on("click", (event) => {
            $(event.currentTarget as Node as Element).addClass("active").siblings().removeClass("active");

            const selectedRoleId = $(event.currentTarget as Node as Element).data("role-id");
            this.loadUsers(selectedRoleId);
            this.loadPermissions(selectedRoleId);
        });

        $("form.role-search-form").on("submit", () => {
            this.clearSections();
        });

        $("#rolePagination a").on("click", () => {
            this.clearSections();
        });

        this.initRemoveRoleButtons();
        this.initEditRoleButtons();
    }

    private loadUsers(roleId: number) {
        const userSection = $("#user-section .section");
        const container = userSection.find(".list-container");
        const searchForm = userSection.find(".user-search-form");
        searchForm.find("input.search-value").val("");
        container.html("<div class=\"loader\"></div>");
        userSection.removeClass("hide");

        this.client.getUsersByRole(roleId).done(response => {
            container.html(response);
            this.initRemoveUserFromRoleButton();
        }).fail((error) => {
            const errorAlert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("ListError", "PermissionJs").value));
            container.empty().append(errorAlert.buildElement());
        }).always(() => {
            this.userList = new ListWithPagination(`Permission/UsersByRole?roleId=${roleId}`,
                "user",
                ViewType.Widget,
                false,
                this.initRemoveUserFromRoleButton,
                this);
            this.userList.init();
            this.userList.setSearchFormDisabled(false);
            $("#addRoleButton").removeClass("disabled");
        });
    }

    private loadPermissions(roleId: number) {
        const permissionSection = $("#permission-section .section");
        const container = permissionSection.find(".list-container");
        container.html("<div class=\"loader\"></div>");
        const searchForm = permissionSection.find(".permission-search-form");
        searchForm.find("input.search").val("");
        permissionSection.removeClass("hide");

        this.client.getPermissionsByRole(roleId).done(response => {
            container.html(response);
            this.initPermissionManaging();
        }).fail((error) => {
            const errorAlert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error, localization.translate("ListError", "PermissionJs").value));
            container.empty().append(errorAlert.buildElement());
        }).always(() => {
            this.permissionList = new ListWithPagination(`Permission/RolePermissionList?roleId=${roleId}`,
                "permission",
                ViewType.Widget,
                false,
                this.initPermissionManaging,
                this);
            this.permissionList.init();
            this.permissionList.setSearchFormDisabled(false);
        });
    }

    private initPermissionManaging() {

        $(".permission-row input[type=checkbox]").on("input", (event) => {
            const addPermission = $(event.currentTarget as Node as HTMLElement).is(":checked");
            const permissionCheckboxInput = $(event.currentTarget as Node as HTMLElement);
            permissionCheckboxInput.prop("checked", !addPermission);
            const permissionRow = permissionCheckboxInput.parents(".permission-row");
            const alert = permissionRow.find(".alert");
            alert.hide();
            const specialPermissionId = permissionRow.data("permission-id");
            const roleId = $(".role-row.active").data("role-id");

            const spinner = permissionRow.find(".loading-spinner");
            spinner.show();
            const successLabel = permissionRow.find(".success");
            successLabel.hide();
            
            let result: JQuery.jqXHR<any>;
            if (addPermission) {
                result = this.client.addSpecialPermissionToRole(roleId, specialPermissionId);
            } else {
                result = this.client.removeSpecialPermissionToRole(roleId, specialPermissionId);
            }

            result.done(() => {
                setTimeout(() => {
                    successLabel.show();
                    permissionCheckboxInput.prop("checked", addPermission);
                }, this.delayForShowResponse);
            }).fail((error) => {
                setTimeout(() => {
                    alert.text(this.errorHandler.getErrorMessage(error, localization.translate("PermissionError", "PermissionJs").value));
                    alert.show();
                }, this.delayForShowResponse);
            }).always(() => {
                setTimeout(() => {
                    spinner.hide();
                }, this.delayForShowResponse);
            });
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-user-from-role").click((event) => {
            const userRow = $(event.currentTarget as Node as HTMLElement).parents(".user-row");
            const userId = userRow.data("user-id");
            const alert = userRow.find(".alert");
            alert.hide();

            const roleId = $(".role-row.active").data("role-id");
            this.client.removeUserFromRole(userId, roleId).done(() => {
                this.userList.reloadPage();
            }).fail((error) => {
                alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveUserFromRoleError", "PermissionJs").value));
                alert.show();
            });
        });
    }

    private initEditRoleButtons() {
        const editRoleDialog = $("#editRoleDialog");
        $(".edit-role").click((event) => {
            event.stopPropagation();
            const roleRow = $(event.currentTarget as Node as HTMLElement).parents(".role-row");
            const roleId = roleRow.data("role-id");
            const roleName = roleRow.find(".name").text();
            const roleDescription = roleRow.find(".description").text();

            editRoleDialog.find("input[name=\"Id\"]").val(roleId);
            editRoleDialog.find("input[name=\"Name\"]").val(roleName);
            editRoleDialog.find("input[name=\"Description\"]").val(roleDescription);
            editRoleDialog.find(".form-group").removeClass("has-error");

            editRoleDialog.find(".alert-success").remove();
            const errorSummary = editRoleDialog.find(".validation-summary-errors");
            errorSummary.switchClass("validation-summary-errors", "validation-summary-valid").empty();
            if (errorSummary.length) {
                errorSummary.switchClass("validation-summary-errors", "validation-summary-valid").empty();
            }

            editRoleDialog.modal("show");
        });
    }

    private initEditRoleForm() {
        let editRoleForm = $("#editRoleForm");

        editRoleForm.on("submit", (event) => {
            event.preventDefault();
            event.stopPropagation();
    
            if (editRoleForm.valid()) {
                const editRoleSection = $("#editRoleSection");
                this.client.editRole(editRoleForm.serialize())
                    .done((response) => {
                        editRoleSection.html(response);
                        editRoleForm = $("#editRoleForm");
                        if (editRoleForm.find(".alert-success").length) {
                            this.roleList.reloadPage();
                            this.clearSections();
                        }
                    })
                    .fail((error) => {
                        editRoleSection.html(error.responseText);
                    }).always(() => {
                        this.initEditRoleForm();
                    });
            }
        });
    }


    private initRemoveRoleButtons() {
        $(".remove-role").click((event) => {
            event.stopPropagation();
            const roleRow = $(event.currentTarget as Node as HTMLElement).parents(".role-row");
            const roleName = roleRow.find(".name").text();
            bootbox.dialog({
                title: localization.translate("Warning", "PermissionJs").value,
                message: localization.translateFormat("DeleteRoleConfirm", [roleName],"PermissionJs").value,
                buttons: {
                    confirm: {
                        label: localization.translate("Delete", "PermissionJs").value,
                        className: "btn-default",
                        callback: () => {
                            const roleId = roleRow.data("role-id");
                            const alert = roleRow.find(".alert");
                            alert.hide();
                            this.clearSections();
                            this.client.deleteRole(roleId).done(() => {
                                this.roleList.reloadPage();
                            }).fail((error) => {
                                alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveRoleError", "PermissionJs").value));
                                alert.show();
                            });
                        }
                    },
                    cancel: {
                        label: localization.translate("Cancel", "PermissionJs").value,
                        className: "btn-default",
                        callback: () => {}
                    }
                }
            });
        });
    }

    private initSearchBox() {
        this.searchBox.setDataSet("User");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectionConfirmed) {
                this.currentUserSelectedItem = this.searchBox.getValue();
                const userBox = $("#selectedUser");
                const name = this.getFullNameString(this.currentUserSelectedItem);
                userBox.text(name);
                userBox.data("user-id", this.currentUserSelectedItem.id);
            }
        });

        const addUserToRoleBtn = $("#add-user-to-role");
        if (addUserToRoleBtn.data("init") === false) {
            addUserToRoleBtn.data("init", true);

            const initModalBtn = $("#addRoleButton");
            const addToRoleDialog = $("#addToRoleDialog");
            initModalBtn.click(() => {
                if (!initModalBtn.hasClass("disabled")) {
                    const role = $(".role-row.active");
                    $("#specificRoleName").text(role.find(".name").text());
                    $("#specificRoleDescription").text(role.find(".description").text());
                    addToRoleDialog.modal();
                }
            });

            addToRoleDialog.on("hidden.bs.modal", () => {
                this.currentUserSelectedItem = null; 
                addToRoleDialog.find("#mainSearchInput").val("");
                addToRoleDialog.find("#selectedUser").text(localization.translate("UserIsNotSelected", "Permission").value);
            });

            addUserToRoleBtn.click(() => {
                const roleError = $("#add-user-to-role-error");
                roleError.empty();
                const roleId = $(".role-row.active").data("role-id");
                let userId: number;

               if (typeof this.currentUserSelectedItem == "undefined" || this.currentUserSelectedItem == null)
                    userId = $("#selectedUser").data("user-id");
                else {
                    userId = this.currentUserSelectedItem.id;
                }

                this.client.addUserToRole(userId, roleId).done(() => {
                    this.userList.reloadPage();
                    addToRoleDialog.modal("hide");
                }).fail((error) => {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error, localization.translate("AddUserToRoleError", "PermissionJs").value));
                    roleError.empty().append(errorAlert.buildElement());
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
            var roleError = $("#create-role-error");
            roleError.empty();
            this.client.createRole(roleName, roleDescription).done(() => {
                $("#createRoleModal").modal("hide");
                this.roleList.reloadPage();
                this.clearSections();
            }).fail((error) => {
                const errorAlert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error, localization.translate("CreateRoleError", "PermissionJs").value));
                roleError.empty().append(errorAlert.buildElement());
            });
        });
    }

    private clearSections() {
        this.userList.clear(localization.translate("RoleIsNotSelected", "PermissionJs").value);
        this.permissionList.clear(localization.translate("RoleIsNotSelected", "PermissionJs").value);
        $("#addRoleButton").addClass("disabled");
    }
}