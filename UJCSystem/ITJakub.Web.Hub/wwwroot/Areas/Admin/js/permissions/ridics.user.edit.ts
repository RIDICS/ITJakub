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
    private roleSearchCurrentSelectedItem: IRole;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.roleSearchBox = new SingleSetTypeaheadSearchBox<IRole>("#roleSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));

        this.userId = Number(getQueryStringParameterByName("userId"));
        this.roleList = new ListWithPagination(`Permission/GetRolesByUser?userId=${this.userId}`,
            10,
            "role",
            ViewType.Partial,
            false,
            this.initRemoveUserFromRoleButton,
            this);
        this.roleList.init();
        this.initRemoveUserFromRoleButton();
        this.client = new PermissionApiClient();
        this.errorHandler = new ErrorHandler();
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

        $("#addGroupButton").click(() => {
            $("#addToGroupDialog").modal();
        });

        $("#add-user-to-group").click(() => {
            if ($("#tab2-select-existing").hasClass("active")) {
                let roleId: number;
                const alertHolder = $("#add-user-to-role-error");
                alertHolder.empty();
                if (typeof this.roleSearchCurrentSelectedItem == "undefined")
                    roleId = $("#selectedUser").data("role-id");
                else {
                    roleId = this.roleSearchCurrentSelectedItem.id;
                }
                this.client.addUserToRole(this.userId, roleId).done(() => {
                    $("#addToGroupDialog").modal("hide");
                    this.roleList.reloadPage();
                    this.initRemoveUserFromRoleButton();
                }).fail((error) => {
                    const errorAlert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(this.errorHandler.getErrorMessage(error, localization.translate("AddUserToRoleError", "PermissionJs").value));
                    alertHolder.empty().append(errorAlert.buildElement());
                });
            } else {
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
                });
            }
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-role").click((event) => {
            const roleRow = $(event.currentTarget as Node as HTMLElement).parents(".role-row");
            const roleId = roleRow.data("role-id");
            const alert = roleRow.find(".alert");
            alert.hide();

            this.client.removeUserFromRole(this.userId, roleId).done(() => {
                this.roleList.reloadPage();
            }).fail((error) => {
                alert.text(this.errorHandler.getErrorMessage(error, localization.translate("RemoveUserFromRoleError", "PermissionJs").value));
                alert.show();
            });
        });
    }
}