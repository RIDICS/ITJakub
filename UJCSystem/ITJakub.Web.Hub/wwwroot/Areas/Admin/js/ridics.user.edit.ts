$(document.documentElement).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
});

//TODO rename to role
class UserPermissionEditor {
    private mainContainer: string;
    private roleSearchBox: SingleSetTypeaheadSearchBox<IGroup>;
    private roleSearchCurrentSelectedItem: IGroup;
    private roleList: ListWithPagination;
    private client: WebHubApiClient;
    private userId: number;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.roleSearchBox = new SingleSetTypeaheadSearchBox<IGroup>("#groupSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));

        this.userId = Number(getQueryStringParameterByName("userId"));
        this.roleList = new ListWithPagination(`Permission/GetRolesByUser?userId=${this.userId}`, 10, "role", ViewType.Partial, false, this.initRemoveUserFromRoleButton, this);
        this.roleList.init();
        this.initRemoveUserFromRoleButton();
        this.client = new WebHubApiClient();
    }

    public make() {
        $(this.mainContainer).empty();

        this.roleSearchBox.setDataSet("Role");
        this.roleSearchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectionConfirmed) {
                var selectedItem = this.roleSearchBox.getValue();
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
                var roleId: number;
                if (typeof this.roleSearchCurrentSelectedItem == "undefined")
                    roleId = $("#selectedUser").data("role-id");
                else {
                    roleId = this.roleSearchCurrentSelectedItem.id;
                }
                this.client.addUserToRole(this.userId, roleId).then(() => {
                    $("#addToGroupDialog").modal("hide");
                    this.roleList.reloadPage();
                    this.initRemoveUserFromRoleButton();
                });
            } else {
                var roleName = $("#new-group-name").val() as string;
                var roleDescription = $("#new-group-description").val() as string;
                this.client.createRoleWithUser(this.userId, roleName, roleDescription).then(() => {
                    $("#addToGroupDialog").modal("hide");
                    this.roleList.reloadPage();
                });
            }
        });
    }

    private initRemoveUserFromRoleButton() {
        $(".remove-role").click((event) => {
            var userId = Number(getQueryStringParameterByName("userId"));
            var roleId = $((event.currentTarget) as any).parents("tr.role-row").data("role-id");
            this.client.removeUserFromRole(userId, roleId).then(() => {
                this.roleList.reloadPage();
            });
        });
    }
}
