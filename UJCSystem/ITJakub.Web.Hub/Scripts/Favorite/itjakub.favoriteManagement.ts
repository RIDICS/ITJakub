$(document).ready(() => {
    var favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
    var favoriteManagement = new FavoriteManagement(favoriteManager);
    favoriteManagement.init();
});

class FavoriteManagement {
    private favoriteManager: FavoriteManager;
    private activeLabelId: number;

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

        $("#new-favorite-label-dialog .save-button").click(this.saveNewFavoriteLabel.bind(this));

        $(".favorite-label-management a").click((event) => {
            $(".favorite-label-management").removeClass("active");
            var activeElement = $(event.target).closest(".favorite-label-management");
            activeElement.addClass("active");
            this.activeLabelId = activeElement.data("id");
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

    private saveNewFavoriteLabel() {
        var name = $("#favorite-label-name").val();
        var color = $("#favorite-label-color").val();

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
        $(nameDiv).text(this.name);
        $(nameLink).attr("href", "#")
            .append(nameDiv);
        $(nameColumn)
            .addClass("col-md-10 col-xs-8")
            .append(nameLink);

        var removeColumn = document.createElement("div");
        var removeLink = document.createElement("a");
        var removeIcon = document.createElement("span");
        $(removeIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-remove");
        $(removeLink)
            .attr("href", "#")
            .attr("title", "Smazat oblíbenou položku")
            .append(removeIcon);
        $(removeColumn)
            .addClass("col-md-1 col-xs-2")
            .append(removeLink);

        $(innerContainerDiv)
            .addClass("row")
            .append(iconColumn)
            .append(nameColumn)
            .append(removeColumn);

        this.container
            .append(innerContainerDiv)
            .append(separatorHr);
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