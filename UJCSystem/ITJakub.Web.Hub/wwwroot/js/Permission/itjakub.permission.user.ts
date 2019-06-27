$(document.documentElement).ready(() => {
    var permissionEditor = new UserPermissionEditor("#mainContainer");
    permissionEditor.make();
    var userId = getQueryStringParameterByName("userId");
    if (typeof userId !== "undefined" && userId !== null && userId !== "") {
        permissionEditor.loadUserById(parseInt(userId));
    }
});

class UserPermissionEditor {
    private mainContainer: string;
    private searchBox: SingleSetTypeaheadSearchBox<IUserDetail>;
    private groupSearchBox: SingleSetTypeaheadSearchBox<IGroup>;
    private currentUserSelectedItem: IUserDetail;
    private groupSearchCurrentSelectedItem: IGroup;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new SingleSetTypeaheadSearchBox<IUserDetail>("#mainSearchInput", "Permission",
            this.getFullNameString,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(this.getFullNameString(item), item.email));
        this.groupSearchBox = new SingleSetTypeaheadSearchBox<IGroup>("#groupSearchInput", "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));
    }

    private getFullNameString(user: IUser): string {
        return user.userName + " - " + user.firstName + " " + user.lastName;
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
                var selectedItem = this.searchBox.getValue();
                this.loadUser(selectedItem);
            }
        });

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

                        this.loadUser(this.currentUserSelectedItem);
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

                        this.loadUser(this.currentUserSelectedItem);
                    }
                });
            }

        });
    }

    private loadUser(user: IUserDetail) {
        this.currentUserSelectedItem = user;
        updateQueryStringParameter("userId", user.id);

        $("#selected-item-div").removeClass("hidden");
        $("#specificUserFirstName").text(user.firstName);
        $("#specificUserLastName").text(user.lastName);
        $("#specificUserUsername").text(user.userName);
        $("#specificUserEmail").text(user.email);

        var groupsUl = $("ul.items-list");
        $(groupsUl).empty();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetRolesByUser",
            data: { userId: user.id } as JQuery.PlainObject,
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
            data: { userId: userId } as JQuery.PlainObject,
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
        editAnchor.href = "/Permission/EditGroup?groupId=" + group.id;

        var editSpan = document.createElement("span");
        $(editSpan).addClass("glyphicon glyphicon-edit list-item-edit");

        editAnchor.appendChild(editSpan);

        buttonsSpan.appendChild(editAnchor);

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            this.removeGroup(group.id);
        });

        buttonsSpan.appendChild(removeSpan);

        groupLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: JQuery.Event) => {
            const target = $(event.target as Node as Element);
            var detailsDiv = $(target).parents(".list-item").first().find(".list-item-details").first();
            if (detailsDiv.is(":hidden")) {
                target.removeClass("glyphicon-chevron-down");
                target.addClass("glyphicon-chevron-up");
                detailsDiv.slideDown();
            } else {
                target.removeClass("glyphicon-chevron-up");
                target.addClass("glyphicon-chevron-down");
                detailsDiv.slideUp();
            }
        });

        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");

        moreSpan.appendChild(iconSpan);

        groupLi.appendChild(moreSpan);

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = group.name;

        groupLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");
        detailsDiv.innerHTML = group.description;

        $(detailsDiv).hide();

        groupLi.appendChild(detailsDiv);

        return groupLi;
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

                this.loadUser(this.currentUserSelectedItem);
            }
        });

    }
}
