$(document).ready(() => {
    var favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
    var favoriteManagement = new FavoriteManagement(favoriteManager);
    favoriteManagement.init();
});

class FavoriteManagement {
    private favoriteManager: FavoriteManager;
    private activeLabelId: number;
    private activeLabelForEditing: JQuery;

    constructor(favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.activeLabelForEditing = null;
    }

    public init() {
        $("#favorite-label-color").colorpickerplus();
        $("#favorite-label-color").on("changeColor", (event, color) => {
            if (color == null) {
                $(event.target)
                    .val("#FFFFFF")
                    .css("background-color", "#FFFFFF");
            } else {
                $(event.target)
                    .val(color)
                    .css("background-color", color);
            }
        });

        $("#add-new-label").click(() => {
            this.showAddLabelDialog();
        });

        $("#new-favorite-label-dialog .save-button").click(() => {
            this.saveFavoriteLabel();
        });

        $(".favorite-label-management .favorite-label-link").click((event) => {
            $(".favorite-label-management").removeClass("active");
            var activeElement = $(event.target).closest(".favorite-label-management");
            activeElement.addClass("active");
            this.activeLabelId = activeElement.data("id");
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

        this.loadFavoriteItems();
    }

    private loadFavoriteItems() {
        // todo get all sort and filter parameters

        var container = $("#favorite-item-container");
        container.empty();

        this.favoriteManager.getFavorites((favorites) => {
            for (let i = 0; i < favorites.length; i++) {
                var favoriteItem = favorites[i];
                var item = new FavoriteManagementItem(container, favoriteItem.FavoriteType, favoriteItem.Id, favoriteItem.Title, favoriteItem.CreateTime);
                item.make();
            }
        });
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
        $("#favorite-label-name").val(name);
        $("#favorite-label-color").val(color);
        $("#new-favorite-label-dialog").modal("show");
    }

    private removeLabel(item: JQuery) {
        //todo request to server

        // todo after save remove from UI:
        item.remove();
        $("#remove-dialog").modal("hide");
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

            $(labelDiv).load(url);
        });
    }

    private saveEditedFavoriteLabel(labelItem: JQuery, name: string, color: string) {
        // todo send to server

        // todo update correctly UI:
        $("#new-favorite-label-dialog").modal("hide");

        $(".favorite-label-name", labelItem).text(name);
        labelItem.css("background-color", color);
        labelItem.data("name", name);
        labelItem.data("color", color);
    }

    private saveFavoriteLabel() {
        var name = $("#favorite-label-name").val();
        var color = $("#favorite-label-color").val();
        
        if (this.activeLabelForEditing == null) {
            this.saveNewFavoriteLabel(name, color);
        } else {
            this.saveEditedFavoriteLabel(this.activeLabelForEditing, name, color);
        }
    }
}

class FavoriteManagementItem {
    private createTime: string;
    private name: string;
    private id: number;
    private type: FavoriteType;
    private container: JQuery;
    private innerContainerDiv: HTMLDivElement;
    private separatorHr: HTMLHRElement;

    constructor(container: JQuery, type: FavoriteType, id: number, name: string, createTime: string) {
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
            .addClass("glyphicon-remove");
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
        // TODO send request to server
        $("#remove-dialog").modal("hide");

        // todo after success remove from UI:
        $(this.innerContainerDiv).remove();
        $(this.separatorHr).remove();
    }

    private edit() {
        var newName = $("#favorite-item-name").val();

        //todo send request to server
        $("#edit-favorite-dialog").modal("hide");

        // todo after success update UI:
        this.name = newName;
        $(".favorite-item-name", this.innerContainerDiv).text(newName);

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
