$(document).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
});

class UserPermissionEditor {
    private mainContainer: string;
    private searchBox: ConcreteInstanceSearchBox;
    private groupSearchBox: ConcreteInstanceSearchBox;
    private currentUserSelectedItem: IUser;
    private groupSearchCurrentSelectedItem: IGroup;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new ConcreteInstanceSearchBox("#mainSearchInput", "Permission", this.getPrintableItem );
        this.groupSearchBox = new ConcreteInstanceSearchBox("#groupSearchInput", "Permission", this.getGroupPrintableItem );
    }

    public getPrintableItem(item: IUser): IPrintableItem {
        var printableUser: IPrintableItem = {
            Name: item.UserName+" - "+item.FirstName+" "+item.LastName,
            Description: item.Email
        };
        return printableUser;
    }

    public getGroupPrintableItem(item: IGroup): IPrintableItem {
        var printableGroup: IPrintableItem = {
            Name: item.Name,
            Description: item.Description
        };
        return printableGroup;
    }

    public make() {
        $(this.mainContainer).empty();
        this.searchBox.setDataSet("User");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
             
            if (!selectedExists || this.searchBox.getInputValue() === "") {
                $("#right-panel").addClass("hidden");
                $("#selected-item-div").addClass("hidden");
            }

            if (selectionConfirmed) {
                var selectedItem = <IUser>this.searchBox.getValue();
                this.loadUser(selectedItem);
            }
        });

        this.groupSearchBox.setDataSet("Group");
        this.groupSearchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {

            if (!selectedExists || this.searchBox.getInputValue() === "") {
            }

            if (selectionConfirmed) {
                var selectedItem = <IGroup>this.groupSearchBox.getValue();
                this.groupSearchCurrentSelectedItem = selectedItem;
                $("#specificGroupName").text(selectedItem.Name);
                $("#specificGroupDescription").text(selectedItem.Description);
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
                    data: JSON.stringify({ userId: this.currentUserSelectedItem.Id, groupId: this.groupSearchCurrentSelectedItem.Id }),
                    dataType: "json",
                    contentType: "application/json",
                    success: (response) => {

                        $("#addToGroupDialog").modal('hide');

                        this.loadUser(this.currentUserSelectedItem);
                    }
                });

            } else {

                var groupName = $("#new-group-name").val();
                var groupDescription = $("#new-group-description").val();

                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: getBaseUrl() + "Permission/CreateGroupWithUser",
                    data: JSON.stringify({ userId: this.currentUserSelectedItem.Id, groupName: groupName, groupDescription: groupDescription }),
                    dataType: "json",
                    contentType: "application/json",
                    success: (response) => {

                        $("#addToGroupDialog").modal('hide');

                        this.loadUser(this.currentUserSelectedItem);
                    }
                });
            }

        });
    }

    private loadUser(user: IUser) {
        this.currentUserSelectedItem = user;
        $("#selected-item-div").removeClass("hidden");
        $("#specificUserFirstName").text(user.FirstName);
        $("#specificUserLastName").text(user.LastName);
        $("#specificUserUsername").text(user.UserName);
        $("#specificUserEmail").text(user.Email);

        var groupsUl = $("ul.user-groups");
        $(groupsUl).empty();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetGroupsByUser",
            data: { userId: user.Id },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {

                var results = response["result"];
                 
                for (var i = 0; i < results.length; i++) {
                    var group = results[i];

                    var groupLi = document.createElement("li");
                    groupLi.innerHTML = group["Name"];

                    groupsUl.append(groupLi);
                }

                $("#right-panel").removeClass("hidden");
            }
        });
    }
}
