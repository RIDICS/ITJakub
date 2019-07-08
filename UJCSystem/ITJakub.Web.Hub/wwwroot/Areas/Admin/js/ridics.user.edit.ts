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
    private userId: number;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.roleSearchBox = new SingleSetTypeaheadSearchBox<IGroup>("#groupSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));

        this.userId = Number(getQueryStringParameterByName("userId"));
        this.roleList = new ListWithPagination(`Permission/GetRolesByUser?userId=${this.userId}`, 10, "role", ViewType.Partial, false, this.initRemoveUserFromRoleButton);
        this.roleList.init();
        this.initRemoveUserFromRoleButton();
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
                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: getBaseUrl() + "Permission/AddUserToRole",
                    data: JSON.stringify({ userId: this.userId, roleId: roleId }),
                    dataType: "json",
                    contentType: "application/json",
                    success: () => {
                        $("#addToGroupDialog").modal("hide");
                        this.roleList.reloadPage();
                        this.initRemoveUserFromRoleButton();
                    }
                });
            } else {
                var roleName = $("#new-group-name").val() as string;
                var roleDescription = $("#new-group-description").val() as string;

                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: getBaseUrl() + "Permission/CreateRoleWithUser",
                    data: JSON.stringify({ userId: this.userId, roleName: roleName, roleDescription: roleDescription }),
                    dataType: "json",
                    contentType: "application/json",
                    success: () => {
                        $("#addToGroupDialog").modal("hide");
                        this.roleList.reloadPage();
                    }
                });
            }
        });
    }

    private initRemoveUserFromRoleButton(list?: ListWithPagination) {
        $(".remove-role").click((event) => {
            var userId = getQueryStringParameterByName("userId");
            var roleId = $((event.currentTarget) as any).parents("tr.role-row").data("role-id");
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/RemoveUserFromRole",
                data: JSON.stringify({ userId: userId, roleId: roleId }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    if (typeof this.roleList != "undefined") {
                        this.roleList.reloadPage();
                    }
                    if (typeof list != "undefined") {
                        list.reloadPage();
                    }
                }
            });
        });
    }
}
