$(document).ready(() => {
    var permissionEditor = new GroupPermissionEditor("#mainContainer");
    permissionEditor.make();
    var groupId = getQueryStringParameterByName("groupId");
    if (typeof groupId !== "undefined" && groupId !== null) {
        permissionEditor.loadGroupById(parseInt(groupId));
    }
});

class GroupPermissionEditor {
    private mainContainer: string;
    private searchBox: ConcreteInstanceSearchBox;
    private groupSearchBox: ConcreteInstanceSearchBox;
    private currentGroupSelectedItem: IGroup;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
    }

    public getPrintableItem(item: IGroup): IPrintableItem {
        var printableGroup: IPrintableItem = {
            Name: item.Name,
            Description: item.Description
        };
        return printableGroup;
    }

    private searchboxStateChangedCallback(selectedExists: boolean, selectionConfirmed: boolean) {
             
        if (!selectedExists || this.searchBox.getInputValue() === "") {
            $("#right-panel").addClass("hidden");
            $("#selected-item-div").addClass("hidden");
            $("#createGroupButton").removeClass("hidden");
        }

        if (selectedExists) {
            $("#createGroupButton").addClass("hidden");
        }

        if (selectionConfirmed) {
            var selectedItem = <IGroup>this.searchBox.getValue();
            this.loadGroup(selectedItem);
        }
    }

    private createSearchbox() {
        if (typeof this.searchBox !== "undefined" && this.searchBox !== null) {
            this.searchBox.clearCache();
        }
        this.searchBox = new ConcreteInstanceSearchBox("#mainSearchInput", "Permission", this.getPrintableItem);
        this.searchBox.setDataSet("Group");

        this.searchBox.create((selectedExist, selectionConfirmed) => {
            this.searchboxStateChangedCallback(selectedExist, selectionConfirmed);
        });
    }

    public make() {
        $(this.mainContainer).empty();

        this.createSearchbox();

        $("#createGroupButton").click(() => {
            $("#new-group-name").val(this.searchBox.getInputValue());
            $("#createGroupModal").modal();
        });

        $("#save-group").click(() => {
            var groupName = $("#new-group-name").val();
            var groupDescription = $("#new-group-description").val();

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/CreateGroup",
                data: JSON.stringify({ groupName: groupName, groupDescription: groupDescription }),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    this.createSearchbox();

                    $("#createGroupModal").modal('hide');

                    this.currentGroupSelectedItem = response["group"];
                    this.loadGroup(this.currentGroupSelectedItem);
                }
            });
        });
    }

    private loadGroup(group: IGroup) {
        this.currentGroupSelectedItem = group;
        updateQueryStringParameter("groupId", group.Id);

        $("#createGroupButton").addClass("hidden");
        $("#selected-item-div").removeClass("hidden");
        $("#right-panel").removeClass("hidden");

        $("#specificGroupName").text(group.Name);
        $("#specificGroupDescription").text(group.Description);
    }

    public loadGroupById(groupId: number) {

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetGroup",
            data: { groupId: groupId },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {

                this.loadGroup(response);

            }
        });
    }
}