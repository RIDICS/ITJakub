$(document).ready(() => {
    var permissionEditor = new GroupPermissionEditor("#mainContainer");
    permissionEditor.make();
    var groupId = getQueryStringParameterByName("groupId");
    if (typeof groupId !== "undefined" && groupId !== null && groupId !== "") {
        permissionEditor.loadGroupById(parseInt(groupId));
    }
});

class GroupPermissionEditor {
    private mainContainer: string;
    private searchBox: SingleSetTypeaheadSearchBox<IGroup>;
    private currentGroupSelectedItem: IGroup;
    private bookSelector: BooksSelector;
    private specialPermissionSelector: SpecialPermissionsSelector;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
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
            var selectedItem = this.searchBox.getValue();
            this.loadGroup(selectedItem);
        }
    }

    private createSearchbox() {
        if (typeof this.searchBox !== "undefined" && this.searchBox !== null) {
            this.searchBox.clearCache();
            this.searchBox.destroy();
        }
        this.searchBox = new SingleSetTypeaheadSearchBox<IGroup>("#mainSearchInput",
            "Permission",
            (item) => item.name,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.name, item.description));
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

            var allowedBooksUl = $("#allowedBooksList");
            $(allowedBooksUl).empty();
            

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/DeleteGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id}),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {
                    this.createSearchbox();
                    this.loadGroup(null);
                }
            });

        });

        $("#addBookToGroup").click(() => {
            this.bookSelector = new BooksSelector(<HTMLDivElement>document.getElementById("add-books-to-group-form"));
            this.bookSelector.make();
            $("#addBookToGroupDialog").modal();
        });

        $("#add-books-to-group-ok").click(() => {
            var selectedBookIds = this.bookSelector.getSelectedBooksIds();
            var selectedCategoryIds = this.bookSelector.getSelectedCategoriesIds();

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/AddBooksAndCategoriesToGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id, bookIds: selectedBookIds, categoryIds: selectedCategoryIds }),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    $("#addBookToGroupDialog").modal('hide');
                    this.loadGroup(this.currentGroupSelectedItem);
                }
            });
        });



        $("#addSpecialPermissionToGroup").click(() => {
            var specialPermissionDialogBody = <HTMLDivElement>document.getElementById("add-special-permission-to-group-form");
            this.specialPermissionSelector = new SpecialPermissionsSelector(specialPermissionDialogBody);
            this.specialPermissionSelector.make();
            $("#addSpecialPermissionToGroupDialog").modal();
        });

        $("#add-special-permissions-to-group-ok").click(() => {
            var specialPermissionIds = this.specialPermissionSelector.getSelectedSpecialPermissionsIds();

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/AddSpecialPermissionsToGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id, specialPermissionIds: specialPermissionIds}),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    $("#addSpecialPermissionToGroupDialog").modal('hide');
                    this.loadGroup(this.currentGroupSelectedItem);
                }
            });
        });
    }

    private loadGroup(group: IGroup) {
        this.currentGroupSelectedItem = group;

        if (typeof group === "undefined" || group === null) {
            $("#allowedBooksList").empty();
            $("#allowedSpecialPermissionList").empty();
            $("#selected-item-div").addClass("hidden");
            $("#right-panel").addClass("hidden");
            updateQueryStringParameter("groupId", "");
            return;
        }

        updateQueryStringParameter("groupId", group.id);

        $("#createGroupButton").addClass("hidden");
        $("#selected-item-div").removeClass("hidden");
        $("#right-panel").removeClass("hidden");

        $("#specificGroupName").text(group.name);
        $("#specificGroupDescription").text(group.description);

        var allowedBooksUl = $("#allowedBooksList");
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
                    $(item).addClass("root-category");
                    allowedBooksUl.append(item);
                }

                $("#right-panel").removeClass("hidden");
            }
        });

        var allowedSpecialPermissionsUl = $("#allowedSpecialPermissionList");
        $(allowedSpecialPermissionsUl).empty();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetSpecialPermissionsForGroup",
            data: {groupId: group.id},
            dataType: "json",
            contentType: "application/json",
            success: (specialPermissions) => {

                for (var specialPermissionsOfType in specialPermissions) {
                    if (specialPermissions.hasOwnProperty(specialPermissionsOfType)) {
                        var item = this.createSpecialPermissionNodeItem(specialPermissionsOfType, specialPermissions[specialPermissionsOfType]);
                        $(item).addClass("special-permission");
                        allowedSpecialPermissionsUl.append(item);
                    }
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

    private loadCategoryContent(targetDiv, categoryId: number, bookType: BookTypeEnum) {
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetCategoryContent",
            data: { groupId: this.currentGroupSelectedItem.id, categoryId: categoryId, bookType: bookType },
            dataType: "json",
            contentType: "application/json",
            success: (response: ICategoryContent) => {

                var detailsUl: HTMLUListElement = document.createElement("ul");
                $(detailsUl).addClass("list-item-details-list");

                for (var i = 0; i < response.categories.length; i++) {
                    var category = response.categories[i];
                    var item = this.createCategoryListItem(category);
                    detailsUl.appendChild(item);
                }

                for (var i = 0; i < response.books.length; i++) {
                    var book = response.books[i];
                    var bookItem = this.createBookListItem(book);
                    detailsUl.appendChild(bookItem);
                }

                $(targetDiv).append(detailsUl);
                $(targetDiv).addClass("loaded");

            }
        });

    }


    private createCategoryListItem(category: ICategoryOrBookType): HTMLLIElement {
        var groupLi = document.createElement("li");
        $(groupLi).addClass("list-item non-leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/RemoveBooksAndCategoriesFromGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id, bookIds: new Array<number>(), categoryIds: [category.id] }),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    var detailsDiv = $(groupLi).find(".list-item-details").first();
                    $(detailsDiv).empty();
                    $(detailsDiv).removeClass("loaded");

                    var currentRootCategory = $(groupLi);

                    if (!$(currentRootCategory).hasClass("root-category")) {
                        currentRootCategory = $(groupLi).parents(".root-category");
                    }

                    var otherRootCategories = currentRootCategory.siblings(".root-category");
                    for (var i = 0; i < otherRootCategories.length; i++) {
                        var rootCategory = otherRootCategories[i];
                        this.unloadWholeCategory(<HTMLLIElement>rootCategory);
                    }
                }
            });
        });

        //buttonsSpan.appendChild(removeSpan); // removing whole permission group is currently not supported

        groupLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: Event) => {
            var target: JQuery = $(event.target);
            if ($(target).hasClass("list-item-more")) {
                target = $(target).find("span.glyphicon").first();
            }

            var detailsDiv = $(target).parents(".list-item").first().find(".list-item-details").first();

            if (detailsDiv.is(":hidden")) {
                $(target).removeClass("glyphicon-chevron-down");
                $(target).addClass("glyphicon-chevron-up");

                if (!detailsDiv.hasClass("loaded")) {
                    this.loadCategoryContent(detailsDiv, category.id, category.bookType);
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
        nameSpan.innerHTML = category.description;

        groupLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");

        $(detailsDiv).hide();

        groupLi.appendChild(detailsDiv);

        return groupLi;
    }

    private createBookListItem(book: IBook): HTMLLIElement {
        var bookLi = document.createElement("li");
        $(bookLi).addClass("list-item leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/RemoveBooksAndCategoriesFromGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id, bookIds: [book.id], categoryIds: new Array<number>() }),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    var otherRootCategories = $(bookLi).parents(".root-category").siblings(".root-category");
                    $(bookLi).remove();

                    for (var i = 0; i < otherRootCategories.length; i++) {
                        var rootCategory = otherRootCategories[i];
                        this.unloadWholeCategory(<HTMLLIElement>rootCategory);
                    }
                }
            });
        });

        buttonsSpan.appendChild(removeSpan);

        bookLi.appendChild(buttonsSpan);
        
        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = book.title;

        bookLi.appendChild(nameSpan);


        return bookLi;
    }

    private createSpecialPermissionNodeItem(type: string, specialPermissions: Array<ISpecialPermission>): HTMLLIElement {
        var groupSpecialPermissionsLi = document.createElement("li");
        $(groupSpecialPermissionsLi).addClass("list-item non-leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        buttonsSpan.appendChild(removeSpan);

        groupSpecialPermissionsLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: Event) => {
            var target: JQuery = $(event.target);
            if ($(target).hasClass("list-item-more")) {
                target = $(target).find("span.glyphicon").first();
            }

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

        groupSpecialPermissionsLi.appendChild(moreSpan);

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = SpecialPermissionTextResolver.resolveSpecialPermissionCategoryText(type, specialPermissions);

        groupSpecialPermissionsLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");

        var detailsUl: HTMLUListElement = document.createElement("ul");
        $(detailsUl).addClass("list-item-details-list");

        for (var i = 0; i < specialPermissions.length; i++) {
            var specPermission = specialPermissions[i];
            var item = this.createSpecialPermissionLeafItem(type, specPermission);
            detailsUl.appendChild(item);
        }

        $(detailsDiv).append(detailsUl);
        $(detailsDiv).hide();


        $(removeSpan).click(() => {
            $(detailsDiv).find(".list-item-remove").click();
        });

        groupSpecialPermissionsLi.appendChild(detailsDiv);

        return groupSpecialPermissionsLi;
    }

    private createSpecialPermissionLeafItem(type: string, specialPermission: ISpecialPermission): HTMLLIElement {
        var specPermissionLi = document.createElement("li");
        $(specPermissionLi).addClass("list-item leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var removeSpan = document.createElement("span");
        $(removeSpan).addClass("glyphicon glyphicon-trash list-item-remove");

        $(removeSpan).click(() => {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Permission/RemoveSpecialPermissionsFromGroup",
                data: JSON.stringify({ groupId: this.currentGroupSelectedItem.id, specialPermissionIds: [specialPermission.id]}),
                dataType: "json",
                contentType: "application/json",
                success: (response) => {

                    var parentNodeItem: HTMLLIElement = <HTMLLIElement>$(specPermissionLi).parents("li.list-item.non-leaf").first()[0];
                    $(specPermissionLi).remove();
                    this.removeSpecialPermissionNodeItemIfEmpty(parentNodeItem);

                }
            });
        });

        buttonsSpan.appendChild(removeSpan);

        specPermissionLi.appendChild(buttonsSpan);

        specPermissionLi.appendChild(buttonsSpan);

        var textSpan = document.createElement("span");
        $(textSpan).addClass("list-item-name");
        textSpan.innerHTML = SpecialPermissionTextResolver.resolveSpecialPermissionText(type, specialPermission);

        specPermissionLi.appendChild(textSpan);

        return specPermissionLi;
    }

    private removeSpecialPermissionNodeItemIfEmpty(nodeItem: HTMLLIElement) {
        var listItems = $(nodeItem).find(".list-item");
        if (typeof listItems === "undefined" || listItems === null || listItems.length === 0) {
            $(nodeItem).remove();
        }
    }

    private unloadWholeCategory(category: HTMLLIElement) {
        var categoryDetails = $(category).find(".list-item-details").first();
        $(categoryDetails).slideUp();
        $(categoryDetails).empty();
        $(categoryDetails).removeClass("loaded");

        var moreSpanIcon = $(category).children("span.list-item-more").first().children("span.glyphicon");
        $(moreSpanIcon).removeClass("glyphicon-chevron-up");
        $(moreSpanIcon).addClass("glyphicon-chevron-down");
    }
}

class SpecialPermissionTextResolver {
    
    private static newsPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.NewsPermissionContract";
    private static uploadBookPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.UploadBookPermissionContract";
    private static managePermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.ManagePermissionsPermissionContract";
    private static feedbackPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.FeedbackPermissionContract";
    private static cardFilePermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.CardFilePermissionContract";
    private static autoimportPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.AutoImportCategoryPermissionContract";
    private static readLemmatizationPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.ReadLemmatizationPermissionContract";
    private static editLemmatizationPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.EditLemmatizationPermissionContract";
    private static derivateLemmatizationPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.DerivateLemmatizationPermissionContract";
    private static editionPrintPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.EditionPrintPermissionContract";
    private static editStaticTextPermission: string = "Vokabular.MainService.DataContracts.Contracts.Permission.EditStaticTextPermissionContract";
    
    static resolveSpecialPermissionCategoryText(type: string, specialPermissions: ISpecialPermission[]): string {

        switch (type) {
            case this.newsPermission:
                return "Novinky";
            case this.uploadBookPermission:
                return "Nahrávání děl";
            case this.managePermission:
                return "Správa práv";
            case this.feedbackPermission:
                return "Správa připomínek";
            case this.cardFilePermission:
                return "Prohlížení kartoték";
            case this.autoimportPermission:
                return "Automatické právo na kategorii";
            case this.readLemmatizationPermission:
                return "Prohlížení lematizace";
            case this.editLemmatizationPermission:
                return "Úprava lematizace";
            case this.derivateLemmatizationPermission:
                return "Derivace hláskových podob";
            case this.editionPrintPermission:
                return "Tisk edic";
            case this.editStaticTextPermission:
                return "Úprava statických textů";
            default:
                return "Neznámé právo";
        }
         
    }

    static resolveSpecialPermissionText(type: string, specialPermission: ISpecialPermission): string {

        switch (type) {
            case this.newsPermission:
                return "Přidávat novinky";
            case this.uploadBookPermission:
                return "Nahrávat díla";
            case this.managePermission:
                return "Spravovat práva";
            case this.feedbackPermission:
                return "Číst připomínky";
            case this.cardFilePermission:
                return this.resolveCardFileText(<ICardFilePermission>specialPermission);
            case this.autoimportPermission:
                return this.resolveAutoImportText(<IAutoImportPermission>specialPermission);
            case this.readLemmatizationPermission:
                return "Prohlížení lematizace";
            case this.editLemmatizationPermission:
                return "Úprava lematizace";
            case this.derivateLemmatizationPermission:
                return "Derivace hláskových podob";
            case this.editionPrintPermission:
                return "Tisk edic";
            case this.editStaticTextPermission:
                return "Úprava statických textů";
            default:
                return "Neznámé právo";
        }
    }

    private static resolveCardFileText(cardFilePermission: ICardFilePermission):string {
        return cardFilePermission.cardFileName;
    }

    private static resolveAutoImportText(autoimportPermission: IAutoImportPermission): string {
        var label = BookTypeHelper.getText(autoimportPermission.bookType);
        return label;
    }

}

class SpecialPermissionsSelector {
    private container: HTMLDivElement;
    private specialPermissionIds: Array<number>;

    constructor(container: HTMLDivElement) {
        this.container = container;
    }

    public make() {
        this.specialPermissionIds = new Array<number>();

        var specPermissionsUl = document.createElement("ul");
        $(specPermissionsUl).addClass("items-list");

        $(this.container).empty();
        $(this.container).append(specPermissionsUl);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetSpecialPermissions",
            data: {},
            dataType: "json",
            contentType: "application/json",
            success: (specialPermissions) => {

                for (var specialPermissionsOfType in specialPermissions) {
                    if (specialPermissions.hasOwnProperty(specialPermissionsOfType)) {
                        var item = this.createNodeItem(specialPermissionsOfType, specialPermissions[specialPermissionsOfType]);
                        $(item).addClass("special-permission");
                        specPermissionsUl.appendChild(item);
                    }
                }
            }
        });

    }

    private createNodeItem(type: string, specialPermissions: Array<ISpecialPermission>): HTMLLIElement {
        var groupSpecialPermissionsLi = document.createElement("li");
        $(groupSpecialPermissionsLi).addClass("list-item non-leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var checkSpan = document.createElement("span");
        $(checkSpan).addClass("list-item-check");

        var checkInput = document.createElement("input");
        checkInput.type = "checkbox";

        checkSpan.appendChild(checkInput);

        buttonsSpan.appendChild(checkSpan);

        groupSpecialPermissionsLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: Event) => {
            var target: JQuery = $(event.target);
            if ($(target).hasClass("list-item-more")) {
                target = $(target).find("span.glyphicon").first();
            }

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

        groupSpecialPermissionsLi.appendChild(moreSpan);

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = SpecialPermissionTextResolver.resolveSpecialPermissionCategoryText(type, specialPermissions);

        groupSpecialPermissionsLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");

        var detailsUl: HTMLUListElement = document.createElement("ul");
        $(detailsUl).addClass("list-item-details-list");

        for (var i = 0; i < specialPermissions.length; i++) {
            var specPermission = specialPermissions[i];
            var item = this.createLeafItem(type, specPermission);
            detailsUl.appendChild(item);
        }

        $(detailsDiv).append(detailsUl);
        $(detailsDiv).hide();

        $(checkInput).change((event: Event) => {
            var target: HTMLInputElement = <HTMLInputElement>event.target;

            if (target.checked) {
                $(detailsDiv).find(".list-item-check input").prop("checked", true).trigger("change");
            } else {
                $(detailsDiv).find(".list-item-check input").prop("checked", false).trigger("change");
            }

        });

        groupSpecialPermissionsLi.appendChild(detailsDiv);

        return groupSpecialPermissionsLi;
    }

    private createLeafItem(type: string, specialPermission: ISpecialPermission): HTMLLIElement {
        var specPermissionLi = document.createElement("li");
        $(specPermissionLi).addClass("list-item leaf");

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var checkSpan = document.createElement("span");
        $(checkSpan).addClass("list-item-check");

        var checkInput = document.createElement("input");
        checkInput.type = "checkbox";

        $(checkInput).change((event: Event) => {
            var target: HTMLInputElement = <HTMLInputElement>event.target;

            if (target.checked) {
                this.addToSelectedPermissions(specialPermission.id);
            } else {
                this.removeFromSelectedPermissions(specialPermission.id);
            }

            var parentNodeItem: HTMLLIElement = <HTMLLIElement>$(specPermissionLi).parents("li.list-item.non-leaf").first()[0];
            this.changeStateOfNodeItemCheckIfNeeded(parentNodeItem);

        });

        checkSpan.appendChild(checkInput);

        buttonsSpan.appendChild(checkSpan);

        specPermissionLi.appendChild(buttonsSpan);

        var textSpan = document.createElement("span");
        $(textSpan).addClass("list-item-name");
        textSpan.innerHTML = SpecialPermissionTextResolver.resolveSpecialPermissionText(type, specialPermission);

        specPermissionLi.appendChild(textSpan);

        return specPermissionLi;
    }

    private changeStateOfNodeItemCheckIfNeeded(nodeItem: HTMLLIElement) {
        var checked = $(nodeItem).find(".list-item-details .list-item-check input:checked");
        var notChecked = $(nodeItem).find(".list-item-details .list-item-check input:not(:checked)");

        var nodeItemCheckbox = $(nodeItem).children(".list-item-buttons").find(".list-item-check input");

        if (typeof checked === "undefined" || checked === null || checked.length === 0) {
            $(nodeItemCheckbox).prop("indeterminate", false);
            $(nodeItemCheckbox).prop("checked", false);
            return;
        }

        if (typeof notChecked === "undefined" || notChecked === null || notChecked.length === 0) {
            $(nodeItemCheckbox).prop("indeterminate", false);
            $(nodeItemCheckbox).prop("checked", true);
            return;
        }

        $(nodeItemCheckbox).prop("indeterminate", true);
    }

    public getSelectedSpecialPermissionsIds(): Array<number> {
        return this.specialPermissionIds;
    }

    protected addToSelectedPermissions(bookId: number) {
        var isSelected = $.grep(this.specialPermissionIds, (currentBookId: number) => (currentBookId === bookId), false).length !== 0;
        if (!isSelected) {
            this.specialPermissionIds.push(bookId);
        }
    }

    protected removeFromSelectedPermissions(bookId: number) {
        this.specialPermissionIds = $.grep(this.specialPermissionIds, (currentBookId: number) => (currentBookId !== bookId), false);
    }
}

class BooksSelector {
    private container: HTMLDivElement;
    private selectedBooksIds: Array<number>;
    private selectedCategoriesIds: Array<number>;
    
    constructor(container: HTMLDivElement) {
        this.container = container;
    }

    public make() {
        this.selectedBooksIds = new Array<number>();
        this.selectedCategoriesIds = new Array<number>();

        var rootCategoriesUl = document.createElement("ul");
        $(rootCategoriesUl).addClass("items-list");

        $(this.container).empty();
        $(this.container).append(rootCategoriesUl);

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
                    rootCategoriesUl.appendChild(item);
                }
            }
        });

    }

    public getSelectedBooksIds(): Array<number> {
        return this.selectedBooksIds;
    }

    public getSelectedCategoriesIds(): Array<number> {
        return this.selectedCategoriesIds;
    }

    private createCategoryListItem(category: ICategoryOrBookType): HTMLLIElement {
        var groupLi = document.createElement("li");
        $(groupLi).addClass("list-item non-leaf");
        $(groupLi).data("id", category.id);
        $(groupLi).data("bookType", category.bookType);

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var checkSpan = document.createElement("span");
        $(checkSpan).addClass("list-item-check");

        var checkInput = document.createElement("input");
        checkInput.type = "checkbox";

        $(checkInput).change((event: Event, data) => {
            var target: HTMLInputElement = <HTMLInputElement>event.target;
            var listItems = $(groupLi).find(".list-item");

            if (target.checked) {
                $(listItems).find(".list-item-check input").prop("checked", false).trigger("change", [{ propagate: false }]);
                $(listItems).find(".list-item-check input").prop("checked", true);
                $(target).prop("checked", true);
                this.addToSelectedCategories(category.id);
            } else {
                $(listItems).find(".list-item-check input").prop("checked", false);
                $(target).prop("checked", false);
                this.removeFromSelectedCategories(category.id);
            }
            
            if (typeof data === "undefined" || data === null || data.propagate === true) {
                var parentCategoryItem: HTMLLIElement = <HTMLLIElement>$(groupLi).parents("li.list-item.non-leaf").first()[0];
                this.changeStateOfCategoryItemCheckboxIfNeeded(parentCategoryItem);    
            }
        });

        //checkSpan.appendChild(checkInput);

        buttonsSpan.appendChild(checkSpan);

        groupLi.appendChild(buttonsSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("list-item-more");

        $(moreSpan).click((event: Event) => {
            var target: JQuery = $(event.target);
            if ($(target).hasClass("list-item-more")) {
                target = $(target).find("span.glyphicon").first();
            }

            var detailsDiv = $(target).parents(".list-item").first().find(".list-item-details").first();
            
            if (detailsDiv.is(":hidden")) {
                $(target).removeClass("glyphicon-chevron-down");
                $(target).addClass("glyphicon-chevron-up");

                if (!detailsDiv.hasClass("loaded")) {
                    this.loadCategoryContent(detailsDiv, category.id, category.bookType);
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
        nameSpan.innerHTML = category.description;

        groupLi.appendChild(nameSpan);

        var detailsDiv = document.createElement("div");
        $(detailsDiv).addClass("list-item-details");

        $(detailsDiv).hide();

        groupLi.appendChild(detailsDiv);

        return groupLi;
    }

    private createBookListItem(book: IBook): HTMLLIElement {
        var bookLi = document.createElement("li");
        $(bookLi).addClass("list-item leaf");
        $(bookLi).data("id", book.id);

        var buttonsSpan = document.createElement("span");
        $(buttonsSpan).addClass("list-item-buttons");

        var checkSpan = document.createElement("span");
        $(checkSpan).addClass("list-item-check");

        var checkInput = document.createElement("input");
        checkInput.type = "checkbox";

        $(checkInput).change((event: Event, data) => {
            var target: HTMLInputElement = <HTMLInputElement>event.target;

            if (target.checked) {
                this.addToSelectedBooks(book.id);
            } else {
                this.removeFromSelectedBooks(book.id);
            }

            if (typeof data === "undefined" || data === null || data.propagate === true) {
                var parentCategoryItem: HTMLLIElement = <HTMLLIElement>$(bookLi).parents("li.list-item.non-leaf").first()[0];
                //this.changeStateOfCategoryItemCheckboxIfNeeded(parentCategoryItem); // parent checkbox is currently disabled
            }
        });

        checkSpan.appendChild(checkInput);

        buttonsSpan.appendChild(checkSpan);

        bookLi.appendChild(buttonsSpan);

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("list-item-name");
        nameSpan.innerHTML = book.title;

        bookLi.appendChild(nameSpan);


        return bookLi;
    }

    private changeStateOfCategoryItemCheckboxIfNeeded(categoryItem: HTMLLIElement) {
        if (typeof categoryItem === "undefined" || categoryItem === null) {
            return;
        }

        var categoryId = $(categoryItem).data("id");

        var checked = $(categoryItem).find(".list-item-details .list-item-check input:checked");
        var notChecked = $(categoryItem).find(".list-item-details .list-item-check input:not(:checked)");

        var nodeItemCheckbox = $(categoryItem).children(".list-item-buttons").find(".list-item-check input");

        if (typeof checked === "undefined" || checked === null || checked.length === 0) {

            $(nodeItemCheckbox).prop("indeterminate", false);
            $(nodeItemCheckbox).prop("checked", false).trigger("change", [{ propagate: false }]);
            this.removeFromSelectedCategories(categoryId);

        } else if (typeof notChecked === "undefined" || notChecked === null || notChecked.length === 0) {

            $(nodeItemCheckbox).prop("indeterminate", false);
            $(nodeItemCheckbox).prop("checked", true).trigger("change", [{ propagate: false }]);
            this.addToSelectedCategories(categoryId);

        } else {

            $(nodeItemCheckbox).prop("indeterminate", true);    
            this.removeFromSelectedCategories(categoryId);
            $(checked).trigger("change", [{ propagate: false }]);
            $(notChecked).trigger("change", [{ propagate: false }]);
        }

        var parentCategoryItem: HTMLLIElement = <HTMLLIElement>$(categoryItem).parents("li.list-item.non-leaf").first()[0];
        this.changeStateOfCategoryItemCheckboxIfNeeded(parentCategoryItem);
    }

    private loadCategoryContent(targetDiv, categoryId: number, bookType: BookTypeEnum) {

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Permission/GetAllCategoryContent",
            data: { categoryId: categoryId, bookType: bookType },
            dataType: "json",
            contentType: "application/json",
            success: (response: ICategoryContent) => {

                var detailsUl: HTMLUListElement = document.createElement("ul");
                $(detailsUl).addClass("list-item-details-list");

                for (var i = 0; i < response.categories.length; i++) {
                    var category = response.categories[i];
                    var item = this.createCategoryListItem(category);
                    detailsUl.appendChild(item);
                }

                for (var i = 0; i < response.books.length; i++) {
                    var book = response.books[i];
                    var bookItem = this.createBookListItem(book);
                    detailsUl.appendChild(bookItem);
                }

                $(targetDiv).append(detailsUl);
                $(targetDiv).addClass("loaded");

                $(targetDiv).parent(".list-item").children(".list-item-buttons").find(".list-item-check input").trigger("change"); //trigger change for labeling currently added checkboxes

            }
        });

    }



    protected addToSelectedBooks(bookId: number) {
        var isSelected = $.grep(this.selectedBooksIds, (currentBookId: number) => (currentBookId === bookId), false).length !== 0;
        if (!isSelected) {
            this.selectedBooksIds.push(bookId);
        }
    }

    protected removeFromSelectedBooks(bookId: number) {
        this.selectedBooksIds = $.grep(this.selectedBooksIds, (currentBookId: number) => (currentBookId !== bookId), false);
    }

    private addToSelectedCategories(categoryId: number) {
        var isSelected = $.grep(this.selectedCategoriesIds, (currentCategoryId: number) => (currentCategoryId === categoryId), false).length !== 0;
        if (!isSelected) {
            this.selectedCategoriesIds.push(categoryId);
        }
    }

    private removeFromSelectedCategories(categoryId: number) {
        this.selectedCategoriesIds = $.grep(this.selectedCategoriesIds, (currentCategoryId: number) => (currentCategoryId !== categoryId), false);
    }

    
}