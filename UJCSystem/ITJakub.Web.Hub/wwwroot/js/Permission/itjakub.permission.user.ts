$(document).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
    var userId = getQueryStringParameterByName("userId");
    if (typeof userId !== "undefined" && userId !== null && userId !== "") {
        permissionEditor.loadUserById(parseInt(userId));
    }
});

class UserPermissionEditor {
    private mainContainer: string;
    private searchBox: SingleSetTypeaheadSearchBox<IUser>;
    private groupSearchBox: SingleSetTypeaheadSearchBox<IGroup>;
    private currentUserSelectedItem: IUser;
    private groupSearchCurrentSelectedItem: IGroup;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new SingleSetTypeaheadSearchBox<IUser>("#mainSearchInput", "Permission",
            this.getFullNameString,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(this.getFullNameString(item), item.Email));
        this.groupSearchBox = new SingleSetTypeaheadSearchBox<IGroup>("#groupSearchInput", "Permission",
            (item) => item.Name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.Name, item.Description));
    }

    private getFullNameString(user: IUser): string {
        return user.UserName + " - " + user.FirstName + " " + user.LastName;
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
        updateQueryStringParameter("userId", user.Id);

        $("#selected-item-div").removeClass("hidden");
        $("#specificUserFirstName").text(user.FirstName);
        $("#specificUserLastName").text(user.LastName);
        $("#specificUserUsername").text(user.UserName);
        $("#specificUserEmail").text(user.Email);

        var groupsUl = $("ul.items-list");
        $(groupsUl).empty();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetGroupsByUser",
            data: { userId: user.Id },
            dataType: "json",
            contentType: "application/json",
            success: (results) => {
                 
                for (var i = 0; i < results.length; i++) {
                    var group = results[i];
                    var item = this.createListItem(group);
                    groupsUl.append(item);
                }

                $("#right-panel").removeClass("hidden");
            }
        });
    }

    public loadUserById(userId: number) {

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetUser",
            data: { userId: userId },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                
                this.loadUser(response);

            }
        });
    }

    private createListItem(group: IGroup): HTMLLIElement {
        var groupLi = document.createElement("li");
        $(groupLi).addClass("list-item");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var editAnchor = document.createElement("a");
        editAnchor.href = "/Permission/GroupPermission?groupId=" + group.Id;

        var editSpan = document.createElement("span");
        $(editSpan).addClass("glyphicon glyphicon-edit list-item-edit");

        editAnchor.appendChild(editSpan);

        buttonsSpan.appendChild(editAnchor);

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            this.removeGroup(group.Id);
        });

        buttonsSpan.appendChild(removeSpan);

        groupLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: Event) => {
            var target = event.target;
            var detailsDiv = $(target).parents(".list-item").first().find(".list-item-details").first();
            if (detailsDiv.is(":hidden")) {
                $(target).removeClass("glyphicon-chevron-down");
                $(target).addClass("glyphicon-chevron-up");
                detailsDiv.slideDown();
            } else {
                $(target).removeClass("glyphicon-chevron-up");
                $(target).addClass("glyphicon-chevron-down");
                detailsDiv.slideUp();
            }
        });

        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");

        moreSpan.appendChild(iconSpan);

        groupLi.appendChild(moreSpan);

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = group.Name;

        groupLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");
        detailsDiv.innerHTML = group.Description;

        $(detailsDiv).hide();

        groupLi.appendChild(detailsDiv);

        return groupLi;
    }

    private removeGroup(groupId: number) {
    
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Permission/RemoveUserFromGroup",
            data: JSON.stringify({ userId: this.currentUserSelectedItem.Id, groupId: groupId }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {

                this.loadUser(this.currentUserSelectedItem);
            }
        });

    }
}
