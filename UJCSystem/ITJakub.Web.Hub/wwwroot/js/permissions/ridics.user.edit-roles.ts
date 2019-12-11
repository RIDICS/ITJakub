$(document.documentElement).ready(() => {
    var permissionEditor = new UserRolesEditor("#mainContainer");
    permissionEditor.make();
});

class UserRolesEditor {
    private readonly mainContainer: string;
    private readonly roleSearchBox: SingleSetTypeaheadSearchBox<IRole>;
    private readonly userId: number;
    private readonly roleList: ListWithPagination;
    private readonly client: PermissionApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly registeredRoleName: string;
    private roleSearchCurrentSelectedItem: IRole;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.roleSearchBox = new SingleSetTypeaheadSearchBox<IRole>("#roleSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));

        this.userId = Number(getQueryStringParameterByName("userId"));
        this.roleList = new ListWithPagination(`Permission/GetRolesByUser?userId=${this.userId}`,
            "role",
            ViewType.Partial,
            false,
            false,
            this.initRemoveUserFromRoleButton,
            this);
        this.roleList.init();
        this.initRemoveUserFromRoleButton();
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
        this.registeredRoleName = $("#registeredRoleName").data("name");
    }

    make() {
        $(this.mainContainer).empty();

        this.roleSearchBox.setDataSet("Role");
        this.roleSearchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectionConfirmed) {
                const selectedItem = this.roleSearchBox.getValue();
                this.roleSearchCurrentSelectedItem = selectedItem;
                $("#specificGroupName").text(selectedItem.name);
                $("#specificGroupName").data("role-id", this.roleSearchCurrentSelectedItem.id);
                $("#specificGroupDescription").text(selectedItem.description);
            }
        });

        $("#addGroupButton").on("click",() => {
            $("#addToGroupDialog").modal();
        });

        const addUserToGroupButton = $("#add-user-to-group");
        const savingIcon = addUserToGroupButton.find(".saving-icon");
        addUserToGroupButton.on("click", () => {
            if ($("#tab2-select-existing").hasClass("active")) {
                const alertHolder = $("#add-user-to-role-error");
                alertHolder.empty();
                if (typeof this.roleSearchCurrentSelectedItem == "undefined" ||
                    this.roleSearchCurrentSelectedItem == null) {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("RoleIsNotSelected", "PermissionJs").value);
                    alertHolder.empty().append(errorAlert.buildElement());
                }
                else {
                    savingIcon.removeClass("hide");
                    const roleId = this.roleSearchCurrentSelectedItem.id;
                    this.client.addUserToRole(this.userId, roleId).done(() => {
                        $("#addToGroupDialog").modal("hide");
                        this.roleList.reloadPage();
                        this.initRemoveUserFromRoleButton();
                    }).fail((error) => {
                        const errorAlert = new AlertComponentBuilder(AlertType.Error)
                            .addContent(this.errorHandler.getErrorMessage(error, localization.translate("AddUserToRoleError", "PermissionJs").value));
                        alertHolder.empty().append(errorAlert.buildElement());
                    }).always(() => {
                        savingIcon.addClass("hide");
                    });
                }
            } else {
                savingIcon.removeClass("hide");
                const alertHolder = $("#create-role-with-user-error");
                alertHolder.empty();
                const roleName = $("#new-group-name").val() as string;
                const roleDescription = $("#new-group-description").val() as string;
                this.client.createRoleWithUser(this.userId, roleName, roleDescription).done(() => {
                    $("#addToGroupDialog").modal("hide");
                    this.roleList.reloadPage();
                }).fail((error) => {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error, localization.translate("CreateRoleWithUserError", "PermissionJs").value));
                    alertHolder.empty().append(errorAlert.buildElement());
                }).always(() => {
                    savingIcon.addClass("hide");
                });
            }
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-role").on("click", (event) => {
            const roleRow = $(event.currentTarget as Node as HTMLElement).parents(".role-row");
            const roleName = roleRow.find(".name").text().trim();

            if (roleName === this.registeredRoleName) {
                bootbox.dialog({
                    title: localization.translate("Warning", "PermissionJs").value,
                    message: localization.translateFormat("RemoveUserFromRegisteredRole", [roleName], "PermissionJs").value,
                    buttons: {
                        cancel: {
                            label: localization.translate("Cancel", "PermissionJs").value,
                            className: "btn-default",
                            callback: () => {}
                        },
                        confirm: {
                            label: localization.translate("Remove","PermissionJs").value,
                            className: "btn-default",
                            callback: () => {
                                this.removeUserFromRole(roleRow);
                            }
                        }
                    }
                });
            } else {
                this.removeUserFromRole(roleRow);
            }
        });
    }
    
    private removeUserFromRole(roleRowElement: JQuery<HTMLElement>) {
        const roleId = roleRowElement.data("role-id");
        const alert = roleRowElement.find(".alert");
        alert.hide();
        
        this.client.removeUserFromRole(this.userId, roleId).done(() => {
            this.roleList.reloadPage();
        }).fail((error) => {
            alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveUserFromRoleError", "PermissionJs").value));
            alert.show();
        });
    }
}