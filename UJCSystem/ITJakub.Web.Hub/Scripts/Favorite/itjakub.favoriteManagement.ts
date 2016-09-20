$(document).ready(() => {
    var favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
    var favoriteManagement = new FavoriteManagement(favoriteManager);
    favoriteManagement.init();
});

class FavoriteManagement {
    private favoriteManager: FavoriteManager;
    private activeLabelId: number;
    private activeLabelForEditing: JQuery;
    private labelColorInput: ColorInput;

    constructor(favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.activeLabelForEditing = null;
        this.activeLabelId = null;
    }

    public init() {
        this.labelColorInput = new ColorInput($("#favorite-label-color"));
        this.labelColorInput.make();

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
        $("#name-filter").change(this.loadFavoriteItems.bind(this));
        //$("#name-filter-button").click(this.loadFavoriteItems.bind(this));

        $("#show-all-link").click(() => {
            this.setActiveLabel(null);
        });

        this.loadFavoriteItems();
    }

    private initFavoriteLabel(item: JQuery) {
        $(".favorite-label-link", item).click(() => {
            this.setActiveLabel(item);
        });

        $(".favorite-label-management .favorite-label-remove-link").click(() => {
            this.showRemoveDialog(item);
        });

        $(".favorite-label-management .favorite-label-edit-link").click(() => {
            var name = item.data("name");
            var color = item.data("color");
            this.showEditLabelDialog(name, color, item);
        });
    }

    private loadFavoriteItems() {
        var sortOrder = $("#sort-select").val();
        var typeFilter = $("#type-filter-select").val();
        var nameFilter = $("#name-filter").val();
        
        var container = $("#favorite-item-container");
        container.empty();

        this.favoriteManager.getFavorites(this.activeLabelId, typeFilter, nameFilter, sortOrder, (favorites) => {
            for (let i = 0; i < favorites.length; i++) {
                var favoriteItem = favorites[i];
                var item = new FavoriteManagementItem(container, favoriteItem.FavoriteType, favoriteItem.Id, favoriteItem.Title, favoriteItem.CreateTime, this.favoriteManager);
                item.make();
            }
        });
    }

    private setActiveLabel(item: JQuery) {
        $(".favorite-label-management").removeClass("active");

        if (item != null) {
            item.addClass("active");
            this.activeLabelId = item.data("id");
        } else {
            this.activeLabelId = null;
        }
        
        this.loadFavoriteItems();
    }

    private showRemoveDialog(item: JQuery) {
        var labelName = item.data("name");

        $("#remove-dialog .modal-body")
            .text("Opravdu chcete smazat vybraný štítek (" + labelName + ")? Štítek bude smazán včetně všech přiřazených oblíbených položek.");

        $("#remove-dialog .remove-button")
            .off("click")
            .click(() => {
                this.removeLabel(item);
            });

        $("#remove-dialog").modal("show");
    }

    private showAddLabelDialog() {
        this.showEditLabelDialog("", "", null);
    }

    private showEditLabelDialog(name: string, color: string, item: JQuery) {
        this.activeLabelForEditing = item;
        this.labelColorInput.setValue(color);
        $("#favorite-label-name").val(name);
        $("#new-favorite-label-dialog").modal("show");
    }

    private removeLabel(item: JQuery) {
        var labelId = item.data("id");
        this.favoriteManager.deleteFavoriteLabel(labelId, () => {
            item.remove();
            $("#remove-dialog").modal("hide");
        });
    }

    private saveNewFavoriteLabel(name: string, color: string) {
        this.favoriteManager.createFavoriteLabel(name, color, (id) => {
            $("#new-favorite-label-dialog").modal("hide");

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
        this.favoriteManager.updateFavoriteLabel(labelId, name, color, () => {
            $("#new-favorite-label-dialog").modal("hide");

            $(".favorite-label-name", labelItem).text(name);
            labelItem.css("background-color", color);
            labelItem.data("name", name);
            labelItem.data("color", color);
        });
    }

    private saveFavoriteLabel() {
        var name = $("#favorite-label-name").val();
        var color = this.labelColorInput.getValue();
        
        if (this.activeLabelForEditing == null) {
            this.saveNewFavoriteLabel(name, color);
        } else {
            this.saveEditedFavoriteLabel(this.activeLabelForEditing, name, color);
        }
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

    constructor(container: JQuery, type: FavoriteType, id: number, name: string, createTime: string, favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.createTime = createTime;
        this.name = name;
        this.id = id;
        this.type = type;
        this.container = container;
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
            .text(this.name)
            .addClass("favorite-item-name");
        $(nameLink)
            .attr("href", "#")
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
            .attr("title", "Smazat oblíbenou položku")
            .append(removeIconContainer)
            .click(() => {
                $("#remove-dialog .modal-body")
                    .text("Opravdu chcete smazat vybranou oblíbenou položku (" + this.name + ")?");

                $("#remove-dialog .remove-button")
                    .off("click")
                    .click(this.remove.bind(this));

                $("#remove-dialog").modal("show");
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

                $("#edit-favorite-dialog").modal("show");
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

    private remove() {
        this.favoriteManager.deleteFavoriteItem(this.id, () => {
            $("#remove-dialog").modal("hide");

            $(this.innerContainerDiv).remove();
            $(this.separatorHr).remove();
        });
    }

    private edit() {
        var newName = $("#favorite-item-name").val();

        this.favoriteManager.updateFavoriteItem(this.id, newName, () => {
            $("#edit-favorite-dialog").modal("hide");

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
                $(icon).addClass("glyphicon-console")
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

class ColorInput {
    private inputElement: JQuery;

    constructor(inputElement: JQuery) {
        this.inputElement = inputElement;
    }

    public make() {
        this.inputElement.colorpickerplus();
        this.inputElement.on("changeColor", (event, color) => {
            if (color == null) {
                color = "#FFFFFF";
            }

            this.setValue(color);
        });

        this.inputElement.change(() => this.updateBackground());

        // disable saving custom colors:
        $(".colorpickerplus-container button").remove();
        if (window.localStorage) {
            window.localStorage.removeItem("colorpickerplus_custom_colors");
        }
    }

    public setValue(value: string) {
        this.inputElement.val(value);
        this.updateBackground();
    }

    public getValue(): string {
        return this.inputElement.val();
    }

    private updateBackground() {
        var value = this.inputElement.val();
        if (value.length !== 7) {
            value = "#FFFFFF";
        }

        this.inputElement.css("background-color", value);
        this.inputElement.css("color", FavoriteHelper.getFontColor(value));
    }
}