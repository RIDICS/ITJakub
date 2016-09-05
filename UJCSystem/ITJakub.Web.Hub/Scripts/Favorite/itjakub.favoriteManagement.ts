$(document).ready(() => {
    var favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
    var favoriteManagement = new FavoriteManagement(favoriteManager);
    favoriteManagement.init();
});

class FavoriteManagement {
    private favoriteManager: FavoriteManager;

    constructor(favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
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
}

class FavoriteManagementItem {
    private createTime: string;
    private name: string;
    private id: number;
    private type: FavoriteType;
    private container: JQuery;

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
        $(iconContainer)
            .attr("style", "width: 24px; height: 24px; background-color: black; margin-left: auto; margin-right: auto");
        $(iconColumn).append(iconContainer);

        var nameColumn = document.createElement("div");
        var nameLink = document.createElement("a");
        var nameDiv = document.createElement("div");
        $(nameDiv).text(this.name);
        $(nameLink).attr("href", "#")
            .append(nameDiv);
        $(nameColumn).append(nameLink);

        var removeColumn = document.createElement("div");
        var removeLink = document.createElement("a");
        var removeIcon = document.createElement("span");
        $(removeIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-remove");
        $(removeLink).attr("href", "#")
            .append(removeIcon);
        removeColumn.appendChild(removeLink);

        $(innerContainerDiv)
            .append(iconColumn)
            .append(nameColumn)
            .append(removeColumn);

        this.container
            .append(innerContainerDiv)
            .append(separatorHr);
    }

    private getIconElement(): HTMLSpanElement {
        var icon = document.createElement("span");
        $(icon).addClass("glyphicon");

        switch (this.type) {
            case FavoriteType.Book:
                $(icon).addClass("glyphicon-book");
                break;
            case FavoriteType.PageBookmark:
                $(icon).addClass("glyphicon-bookmark");
                break;
            case FavoriteType.Category:
                $(icon).addClass("glyphicon-list");
                break;
            case FavoriteType.Query:
                $(icon).addClass("glyphicon-console");
                break;
            case FavoriteType.BookVersion:
                $(icon).addClass("glyphicon-tags");
                break;
            default:
                $(icon).addClass("glyphicon-question-sign");
                break;
        }
        return icon;
    }
}