﻿$(document).ready(() => {
    var permissionEditor = new GroupPermissionEditor("#mainContainer");
    permissionEditor.make();
    var groupId = getQueryStringParameterByName("groupId");
    if (typeof groupId !== "undefined" && groupId !== null && groupId !== "") {
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
            this.searchBox.destroy();
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
                    this.loadGroup(response);
                }
            });
        });

        $("#deleteGroup").click(() => {

            var allowedBooksUl = $("ul.items-list");
            $(allowedBooksUl).empty();
            

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/DeleteGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.Id}),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {
                    this.createSearchbox();
                    this.loadGroup(null);
                }
            });

        });
    }

    private loadGroup(group: IGroup) {
        this.currentGroupSelectedItem = group;

        if (typeof group === "undefined" || group === null) {
            $("ul.items-list").empty();
            $("#selected-item-div").addClass("hidden");
            $("#right-panel").addClass("hidden");
            updateQueryStringParameter("groupId", "");
            return;
        }

        updateQueryStringParameter("groupId", group.Id);

        $("#createGroupButton").addClass("hidden");
        $("#selected-item-div").removeClass("hidden");
        $("#right-panel").removeClass("hidden");

        $("#specificGroupName").text(group.Name);
        $("#specificGroupDescription").text(group.Description);

        var allowedBooksUl = $("ul.items-list");
        $(allowedBooksUl).empty();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetRootCategories",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (categories) => {

                for (var i = 0; i < categories.length; i++) {
                    var category = categories[i];
                    var item = this.createCategoryListItem(category);
                    allowedBooksUl.append(item);
                }

                $("#right-panel").removeClass("hidden");
            }
        });
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

    private loadCategoryContent(targetDiv, categoryId: number) {
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetAllCategoryContent",
            data: { categoryId: categoryId },
            dataType: "json",
            contentType: "application/json",
            success: (response: ICategoryContent) => {

                var detailsUl: HTMLUListElement = document.createElement("ul");
                $(detailsUl).addClass("list-item-details-list");

                for (var i = 0; i < response.Categories.length; i++) {
                    var category = response.Categories[i];
                    var item = this.createCategoryListItem(category);
                    detailsUl.appendChild(item);
                }

                for (var i = 0; i < response.Books.length; i++) {
                    var book = response.Books[i];
                    var bookItem = this.createBookListItem(book);
                    detailsUl.appendChild(bookItem);
                }

                $(targetDiv).append(detailsUl);
                $(targetDiv).addClass("loaded");

            }
        });

    }


    private createCategoryListItem(category: ICategory): HTMLLIElement {
        var groupLi = document.createElement("li");
        $(groupLi).addClass("list-item non-leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            //TODO
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

                if (!detailsDiv.hasClass("loaded")) {
                    this.loadCategoryContent(detailsDiv, category.Id);
                }

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
        nameSpan.innerHTML = category.Description;

        groupLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");

        $(detailsDiv).hide();

        groupLi.appendChild(detailsDiv);

        return groupLi;
    }

    private createBookListItem(book: IBook): HTMLLIElement {
        var groupLi = document.createElement("li");
        $(groupLi).addClass("list-item leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            //TODO
        });

        buttonsSpan.appendChild(removeSpan);

        groupLi.appendChild(buttonsSpan);
        
        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = book.Title;

        groupLi.appendChild(nameSpan);


        return groupLi;
    }
}