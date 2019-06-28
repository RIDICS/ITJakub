$(document.documentElement).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
    var userId = getQueryStringParameterByName("userId");
    //TODO use instead of data-id
    //TODO merge with ridics.user.edit.ts
});

class UserPermissionEditor {
    private mainContainer: string;
    private searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private groupSearchBox: SingleSetTypeaheadSearchBox<IGroup>;
    private currentUserSelectedItem: IUserDetail;
    private groupSearchCurrentSelectedItem: IGroup;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.groupSearchBox = new SingleSetTypeaheadSearchBox<IGroup>("#groupSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));
    }

    private getFullNameString(user: IUser): string {
        return user.userName + " - " + user.firstName + " " + user.lastName;
    }

    public make() {
        $(this.mainContainer).empty();

        this.groupSearchBox.setDataSet("Role");
        this.groupSearchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {

            if (!selectedExists || this.searchBox.getInputValue() === "") {
            }

            if (selectionConfirmed) {
                var selectedItem = this.groupSearchBox.getValue();
                this.groupSearchCurrentSelectedItem = selectedItem;
                $("#specificGroupName").text(selectedItem.name);
                $("#specificGroupDescription").text(selectedItem.description);
            }
        });

        $("#addGroupButton").click(() => {
            $("#addToGroupDialog").modal();
        });

        $("#add-user-to-group").click(() => {

            if ($("#tab2-select-existing").hasClass("active")) {

                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: getBaseUrl() + "Permission/AddUserToGroup",
                    data: JSON.stringify({ userId: this.currentUserSelectedItem.id, groupId: this.groupSearchCurrentSelectedItem.id }),
                    dataType: "json",
                    contentType: "application/json",
                    success: (response) => {
                        $("#addToGroupDialog").modal('hide');
                        //TODO reload  roles
                    }
                });

            } else {

                var groupName = $("#new-group-name").val() as string;
                var groupDescription = $("#new-group-description").val() as string;

                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: getBaseUrl() + "Permission/CreateRoleWithUser",
                    data: JSON.stringify({ userId: this.currentUserSelectedItem.id, roleName: groupName, roleDescription: groupDescription }),
                    dataType: "json",
                    contentType: "application/json",
                    success: (response) => {
                        $("#addToGroupDialog").modal('hide');
                       //TODO reload  roles
                    }
                });
            }
        });
    }

    private removeGroup(groupId: number) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Permission/RemoveUserFromGroup",
            data: JSON.stringify({ userId: this.currentUserSelectedItem.id, groupId: groupId }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                //TODO reload  roles
            }
        });
    }
}
