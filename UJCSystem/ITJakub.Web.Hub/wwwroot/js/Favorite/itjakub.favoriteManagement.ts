$(document).ready(() => {
    var favoriteManager = new FavoriteManager();
    var favoriteManagement = new FavoriteManagement(favoriteManager);
    favoriteManagement.init();
});

class FavoriteManagement {
    private static increaseBackgroundColorPercent = 80;
    private static pageSize = 50;
    private favoriteManager: FavoriteManager;
    private activeLabelId: number;
    private activeLabelForEditing: JQuery;
    private labelColorInput: ColorInput;
    private pagination: Pagination;
    private paginationOptions: Pagination.Options;
    private newFavoriteLabelDialog: FavoriteManagementDialog;
    private removeDialog: FavoriteManagementDialog;

    private currentSortOrder: number;
    private currentTypeFilter: number;
    private currentNameFilter: string;

    private localization : Localization;

    constructor(favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.activeLabelForEditing = null;
        this.activeLabelId = null;

        this.paginationOptions = {
            container: $("#pagination"),
            pageClickCallback: this.loadFavoriteItemsPage.bind(this)
        }
        this.pagination = new Pagination(this.paginationOptions);

        this.newFavoriteLabelDialog = new FavoriteManagementDialog($("#new-favorite-label-dialog"));
        this.removeDialog = new FavoriteManagementDialog($("#remove-dialog"));

        this.localization = new Localization();
    }

    public init() {
        this.labelColorInput = new ColorInput($("#favorite-label-color"), $("#favorite-label-color-button"));
        this.labelColorInput.make();
        this.labelColorInput.setColorChangedCallback(this.renderNewLabelPreview.bind(this));
        $("#favorite-label-name").change(this.renderNewLabelPreview.bind(this));

        $(".favorite-label-management").each((index, element) => {
            this.renderLabelColor($(element));
        });
        
        $("#add-new-label").click(() => {
            this.showAddLabelDialog();
        });

        $("#new-favorite-label-dialog .save-button").click(() => {
            this.saveFavoriteLabel();
        });

        $(".favorite-label-management .favorite-label-link").click((event) => {
            var activeElement = $(event.target).closest(".favorite-label-management");
            this.setActiveLabel(activeElement);
        });

        $(".favorite-label-management .favorite-label-remove-link").click((event) => {
            var item = $(event.target).closest(".favorite-label-management");
            this.showRemoveDialog(item);
        });

        $(".favorite-label-management .favorite-label-edit-link").click((event) => {
            var item = $(event.target).closest(".favorite-label-management");
            var name = item.data("name");
            var color = item.data("color");
            this.showEditLabelDialog(name, color, item);
        });

        $("#sort-select").change(this.loadFavoriteItems.bind(this));
        $("#type-filter-select").change(this.loadFavoriteItems.bind(this));
        $("#name-filter-button").click(this.loadFavoriteItems.bind(this));
        $("#name-filter").keypress((event) => {
            if (event.keyCode === 13) {
                this.loadFavoriteItems();
            }
        });

        $("#label-name-filter").on("change keyup paste", this.filterLabels.bind(this));

        $("#show-all-link").click(() => {
            $("#label-name-filter")
                .val("")
                .trigger("change");
            this.setActiveLabel(null);
        });

        this.loadFavoriteItems();
    }

    private renderNewLabelPreview() {
        var color = this.labelColorInput.getValue();
        var name = $("#favorite-label-name").val();
        var hexColor = new HexColor(color);

        $("#label-preview").text(name);

        if (hexColor.isValidHexColor()) {
            $("#label-preview")
                .css("background-color", color)
                .css("border-color", FavoriteHelper.getDefaultBorderColor(hexColor))
                .css("color", FavoriteHelper.getDefaultFontColor(hexColor));
        } else {
            $("#label-preview")
                .css("background-color", "#FFFFFF")
                .css("border-color", "#000000")
                .css("color", "#000000");
        }
    }

    private renderLabelColor(element: JQuery) {
        var backgroundColor = $(element).data("color");
        var color = new HexColor(backgroundColor);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);
        var inactiveBackground = color.getIncreasedHexColor(FavoriteManagement.increaseBackgroundColorPercent);
        var inactiveBorder = new HexColor(borderColor).getIncreasedHexColor(FavoriteManagement.increaseBackgroundColorPercent);

        $("a", element).css("color", fontColor);
        $(element)
            .css("border-color", borderColor)
            .data("border-color", borderColor)
            .data("font-color", fontColor)
            .data("inactive-border", inactiveBorder)
            .data("inactive-background", inactiveBackground);
    }

    private initFavoriteLabel(item: JQuery) {
        this.renderLabelColor(item);
        if (this.activeLabelId != null) {
            $("a", item).css("color", FavoriteHelper.getInactiveFontColor());
            var backgroundColor = item.data("inactive-background");
            var borderColor = item.data("inactive-border");
            item.css("background-color", backgroundColor)
                .css("border-color", borderColor);
        }

        $(".favorite-label-link", item).click(() => {
            this.setActiveLabel(item);
        });

        $(".favorite-label-remove-link", item).click(() => {
            this.showRemoveDialog(item);
        });

        $(".favorite-label-edit-link", item).click(() => {
            var name = item.data("name");
            var color = item.data("color");
            this.showEditLabelDialog(name, color, item);
        });
    }

    private filterLabels() {
        this.setInactiveLabel();

        var filterValue = $("#label-name-filter").val().toLocaleLowerCase();
        if (!filterValue) {
            $(".favorite-label-management").show();
            $("#no-label").hide();
            this.setActiveLabel(null);
            return;
        }

        var isAnyVisible = false;
        $(".favorite-label-management").each((index, element) => {
            var item = $(element);
            var name = String(item.data("name")).toLocaleLowerCase();
            var inactiveBackgroundColor = item.data("inactive-background");
            var inactiveBorderColor = item.data("inactive-border");

            item.removeClass("active")
                .css("background-color", inactiveBackgroundColor)
                .css("border-color", inactiveBorderColor);
            $("a", item).css("color", FavoriteHelper.getInactiveFontColor());

            if (name.indexOf(filterValue) !== -1) {
                isAnyVisible = true;
                item.show();
            } else {
                item.hide();
            }
        });

        if (isAnyVisible) {
            $("#no-label").hide();
        } else {
            $("#no-label").show();
        }

        this.paginationOptions.callPageClickCallbackOnInit = false;
        this.pagination.make(0, FavoriteManagement.pageSize);
    }

    private showLoader() {
        var loaderDiv = document.createElement("div");
        $(loaderDiv).addClass("loader");

        $("#favorite-item-container")
            .empty()
            .append(loaderDiv);
    }

    private loadFavoriteItems() {
        this.currentSortOrder = $("#sort-select").val();
        this.currentTypeFilter = $("#type-filter-select").val();
        this.currentNameFilter = $("#name-filter").val();
        
        this.showLoader();

        this.paginationOptions.callPageClickCallbackOnInit = false;
        this.pagination.make(0, FavoriteManagement.pageSize);
        this.favoriteManager.getFavoritesCount(this.activeLabelId, this.currentTypeFilter, this.currentNameFilter, (itemsCount) => {
            this.paginationOptions.callPageClickCallbackOnInit = true;
            this.pagination.make(itemsCount, FavoriteManagement.pageSize);
        });
    }

    private loadFavoriteItemsPage(pageNumber: number) {
        this.showLoader();

        var container = $("#favorite-item-container");
        var count = FavoriteManagement.pageSize;
        var start = (pageNumber - 1) * count;
        this.favoriteManager.getFavorites(this.activeLabelId, this.currentTypeFilter, this.currentNameFilter, this.currentSortOrder, start, count, (favorites) => {
            container.empty();
            for (let i = 0; i < favorites.length; i++) {
                var favoriteItem = favorites[i];
                var item = new FavoriteManagementItem(container, favoriteItem.favoriteType, favoriteItem.id, favoriteItem.title, favoriteItem.createTime, this.favoriteManager);
                item.make();
                item.setOnRemoveCallback(() => this.loadFavoriteItemsPage(pageNumber));
            }
            if (favorites.length === 0) {
                $("#no-results").removeClass("hidden");
            } else {
                $("#no-results").addClass("hidden");
            }
        });
    }

    private setInactiveLabel() {
        $("#no-selected-label").removeClass("hidden");
        $("#no-results").addClass("hidden");
        $("#favorite-item-container").empty();
    }

    private setActiveLabel(item: JQuery) {
        $("#no-selected-label").addClass("hidden");

        $(".favorite-label-management")
            .removeClass("active")
            .each((index, element) => {
                var elementJQuery = $(element);
                let fontColor = FavoriteHelper.getInactiveFontColor();
                let backgroundColor = elementJQuery.data("inactive-background");
                let borderColor = elementJQuery.data("inactive-border");
                if (item == null) {
                    fontColor = elementJQuery.data("font-color");
                    backgroundColor = elementJQuery.data("color");
                    borderColor = elementJQuery.data("border-color");
                }

                elementJQuery
                    .css("background-color", backgroundColor)
                    .css("border-color", borderColor);
                $("a", elementJQuery).css("color", fontColor);
            });

        if (item != null) {
            item.addClass("active");
            let backgroundColor = item.data("color");
            let fontColor = item.data("font-color");
            let borderColor = item.data("border-color");

            item.css("background-color", backgroundColor)
                .css("border-color", borderColor);
            $("a", item).css("color", fontColor);

            this.activeLabelId = item.data("id");
        } else {
            this.activeLabelId = null;
        }
        
        this.loadFavoriteItems();
    }

    private showRemoveDialog(item: JQuery) {
        var labelName = item.data("name");

        $("#remove-dialog .modal-body")
            //.text("Opravdu chcete smazat vybraný štítek (" + labelName + ")? Štítek bude smazán včetně všech přiřazených oblíbených položek.");
            .text(this.localization.translateFormat("DeleteModalText", new Array<string>(labelName)).value);

        $("#remove-dialog .remove-button")
            .off("click")
            .click(() => {
                this.removeLabel(item);
            });

        this.removeDialog.show();
    }

    private showAddLabelDialog() {
        this.showEditLabelDialog("", "", null);
    }

    private showEditLabelDialog(name: string, color: string, item: JQuery) {
        this.activeLabelForEditing = item;
        this.labelColorInput.setValue(color);
        $("#favorite-label-name").val(name);
        $("#label-preview").text("").removeClass("style");
        this.newFavoriteLabelDialog.show();
    }

    private removeLabel(item: JQuery) {
        var labelId = item.data("id");
        this.removeDialog.showSaving();
        this.favoriteManager.deleteFavoriteLabel(labelId, (error) => {
            if (error) {
                this.removeDialog.showError("Chyba při odstraňování štítku");
                return;
            }

            if (item.hasClass("active")) {
                this.setActiveLabel(null);
            }

            item.remove();
            this.removeDialog.hide();
        });
    }

    private saveNewFavoriteLabel(name: string, color: string) {
        this.newFavoriteLabelDialog.showSaving();
        this.favoriteManager.createFavoriteLabel(name, color, (id, error) => {
            if (error) {
                this.newFavoriteLabelDialog.showError("Chyba při ukládání štítku");
                return;
            }

            this.newFavoriteLabelDialog.hide();

            var labelDiv = document.createElement("div");
            $("#favorite-labels").append(labelDiv);

            var url = getBaseUrl() + "Favorite/GetFavoriteLabelManagementPartial?";
            var urlParams = {
                id: id,
                name: name,
                color: color
            }
            url = url + $.param(urlParams);

            $(labelDiv).load(url, null, () => {
                this.initFavoriteLabel($(labelDiv).children());
            });
        });
    }

    private saveEditedFavoriteLabel(labelItem: JQuery, name: string, color: string) {
        var labelId = labelItem.data("id");
        this.newFavoriteLabelDialog.showSaving();
        this.favoriteManager.updateFavoriteLabel(labelId, name, color, (error) => {
            if (error) {
                this.newFavoriteLabelDialog.showError("Chyba při ukládání štítku");
                return;
            }

            this.newFavoriteLabelDialog.hide();

            var hexColor = new HexColor(color);
            $(".favorite-label-name", labelItem).text(name);
            labelItem.css("background-color", color);
            labelItem.css("border-color", FavoriteHelper.getDefaultBorderColor(hexColor));
            labelItem.css("color", FavoriteHelper.getDefaultFontColor(hexColor));
            labelItem.data("name", name);
            labelItem.data("color", color);
        });
    }

    private saveFavoriteLabel() {
        var name = $("#favorite-label-name").val();
        var color = this.labelColorInput.getValue();

        var error = "";
        if (!name) {
            error = "Nebylo zadáno jméno.";
        }
        if (!FavoriteHelper.isValidHexColor(color)) {
            error += " Nesprávný formát barvy (požadovaný formát: #000000).";
        }
        if (error.length > 0) {
            this.newFavoriteLabelDialog.showError(error);
            return;
        }

        if (this.activeLabelForEditing == null) {
            this.saveNewFavoriteLabel(name, color);
        } else {
            this.saveEditedFavoriteLabel(this.activeLabelForEditing, name, color);
        }
    }
}

class FavoriteManagementDialog {
    private dialogJQuery: JQuery;

    constructor(dialogJQuery: JQuery) {
        this.dialogJQuery = dialogJQuery;
    }

    public show() {
        $(".error, .saving-icon").addClass("hidden");
        this.dialogJQuery.modal("show");
    }

    public showSaving() {
        $(".saving-icon", this.dialogJQuery)
            .removeClass("hidden");
        $(".error", this.dialogJQuery)
            .addClass("hidden");
    }

    public showError(text: string) {
        $(".saving-icon", this.dialogJQuery)
            .addClass("hidden");
        $(".error", this.dialogJQuery)
            .text(text)
            .removeClass("hidden");
    }

    public hide() {
        this.dialogJQuery.modal("hide");
    }
}

class FavoriteManagementItem {
    private favoriteManager: FavoriteManager;
    private createTime: string;
    private name: string;
    private id: number;
    private type: FavoriteType;
    private container: JQuery;
    private innerContainerDiv: HTMLDivElement;
    private separatorHr: HTMLHRElement;
    private editFavoriteDialog: FavoriteManagementDialog;
    private removeDialog: FavoriteManagementDialog;
    private onRemoveCallback: (id: number) => void;

    private localization : Localization;

    constructor(container: JQuery, type: FavoriteType, id: number, name: string, createTime: string, favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.createTime = createTime;
        this.name = name;
        this.id = id;
        this.type = type;
        this.container = container;

        this.localization = new Localization();

        this.editFavoriteDialog = new FavoriteManagementDialog($("#edit-favorite-dialog"));
        this.removeDialog = new FavoriteManagementDialog($("#remove-dialog"));
    }

    public make() {
        var innerContainerDiv = document.createElement("div");
        var separatorHr = document.createElement("hr");

        var iconColumn = document.createElement("div");
        var iconContainer = document.createElement("div");
        var icon = this.createIconElement();
        $(iconContainer)
            .attr("style", "text-align: center; font-size: 120%;")
            .append(icon);
        $(iconColumn)
            .addClass("col-md-1 col-xs-2")
            .append(iconContainer);

        var nameColumn = document.createElement("div");
        var nameLink = document.createElement("a");
        var nameDiv = document.createElement("div");
        $(nameDiv)
            .text(this.name != null && this.name !== "" ? this.name : this.localization.translate("NoName", "FavoriteJs").value)
            .addClass("favorite-item-name");
        $(nameLink)
            .attr("href", getBaseUrl() + "Favorite/Favorite?id=" + this.id)
            .append(nameDiv);
        $(nameColumn)
            .addClass("col-md-10 col-xs-8")
            .append(nameLink);

        var removeColumn = document.createElement("div");
        var removeLink = document.createElement("a");
        var removeIconContainer = document.createElement("div");
        var removeIcon = document.createElement("span");
        var editLink = document.createElement("a");
        var editIconContainer = document.createElement("div");
        var editIcon = document.createElement("span");
        $(removeIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-trash");
        $(editIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-pencil");
        $(removeIconContainer)
            .addClass("text-center")
            .attr("style", "float: right; width: 45%;")
            .append(removeIcon);
        $(editIconContainer)
            .addClass("text-center")
            .append(editIcon);
        $(removeLink)
            .attr("href", "#")
            .attr("title", this.localization.translate("DeleteFav", "FavoriteJs").value)
            .append(removeIconContainer)
            .click(() => {
                $("#remove-dialog .modal-body")
                    //.text("Opravdu chcete smazat vybranou oblíbenou položku (" + this.name + ")?");
                    .text(this.localization.translateFormat("DeleteItemModalText", new Array<string>(this.name), "FavoriteJs").value);

                $("#remove-dialog .remove-button")
                    .off("click")
                    .click(this.remove.bind(this));

                this.removeDialog.show();
                
            });
        $(editLink)
            .attr("href", "#")
            .attr("title", "Upravit oblíbenou položku")
            .append(editIconContainer)
            .click(() => {
                $("#favorite-item-name").val(this.name);

                $("#edit-favorite-dialog .save-button")
                    .off("click")
                    .click(this.edit.bind(this));

                this.editFavoriteDialog.show();
            });

        $(removeColumn)
            .addClass("col-md-1 col-xs-2")
            .append(removeLink)
            .append(editLink);
        
        $(innerContainerDiv)
            .addClass("row")
            .append(iconColumn)
            .append(nameColumn)
            .append(removeColumn);

        this.container
            .append(innerContainerDiv)
            .append(separatorHr);

        this.innerContainerDiv = innerContainerDiv;
        this.separatorHr = separatorHr;
    }

    public setOnRemoveCallback(callback: (id: number) => void) {
        this.onRemoveCallback = callback;
    }

    private remove() {
        this.removeDialog.showSaving();
        this.favoriteManager.deleteFavoriteItem(this.id, (error) => {
            if (error) {
                this.removeDialog.showError("Chyba při odstraňování položky");
                return;
            }

            this.removeDialog.hide();

            $(this.innerContainerDiv).remove();
            $(this.separatorHr).remove();

            if (this.onRemoveCallback) {
                this.onRemoveCallback(this.id);
            }
        });
    }

    private edit() {
        var newName = $("#favorite-item-name").val();

        this.editFavoriteDialog.showSaving();
        this.favoriteManager.updateFavoriteItem(this.id, newName, (error) => {
            if (error) {
                this.editFavoriteDialog.showError("Chyba při ukládání položky");
                return;
            }

            this.editFavoriteDialog.hide();

            this.name = newName;
            $(".favorite-item-name", this.innerContainerDiv).text(newName);
        });
    }

    private createIconElement(): HTMLSpanElement {
        var icon = document.createElement("span");
        $(icon).addClass("glyphicon");

        switch (this.type) {
            case FavoriteType.Book:
                $(icon).addClass("glyphicon-book")
                    .attr("title", "Kniha");
                break;
            case FavoriteType.PageBookmark:
                $(icon).addClass("glyphicon-bookmark")
                    .attr("title", "Záložka na stránku v knize");
                break;
            case FavoriteType.Category:
                $(icon).addClass("glyphicon-list")
                    .attr("title", "Kategorie");
                break;
            case FavoriteType.Query:
                $(icon).addClass("glyphicon-search")
                    .attr("title", "Vyhledávací dotaz");
                break;
            case FavoriteType.BookVersion:
                $(icon).addClass("glyphicon-tags")
                    .attr("title", "Verze knihy");
                break;
            default:
                $(icon).addClass("glyphicon-question-sign")
                    .attr("title", "Neznámý typ oblíbené položky");
                break;
        }
        return icon;
    }
}
