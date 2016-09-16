class FavoriteStar {
    private favoriteManager: FavoriteManager;
    private favoriteDialog: NewFavoriteDialog;
    private favoriteDefaultTitle: string;
    private favoriteItemType: FavoriteType;
    private itemId: string;
    private container: JQuery;
    private isItemLabeled: boolean;
    private popoverBuilder: FavoritePopoverBuilder;

    constructor(container: JQuery, type: FavoriteType, itemId: string, favoriteDefaultTitle: string, favoriteDialog: NewFavoriteDialog, favoriteManager: FavoriteManager) {
        this.favoriteManager = favoriteManager;
        this.favoriteDialog = favoriteDialog;
        this.favoriteDefaultTitle = favoriteDefaultTitle;
        this.favoriteItemType = type;
        this.itemId = itemId;
        this.container = container;
        this.isItemLabeled = false;
        this.popoverBuilder = new FavoritePopoverBuilder();
    }

    private createGlyphIcon(name: string): HTMLSpanElement {
        var span = document.createElement("span");
        $(span).addClass("glyphicon")
            .addClass(name);

        return span;
    }

    public addFavoriteItems(items: Array<IFavoriteBaseInfo>) {
        for (var i = 0; i < items.length; i++) {
            this.popoverBuilder.addFavoriteItem(items[i]);
        }
        this.isItemLabeled = items.length > 0;
    }

    public addFavoriteLabels(labels: Array<IFavoriteLabel>) {
        for (var i = 0; i < labels.length; i++) {
            this.popoverBuilder.addFavoritLabel(labels[i]);
        }
    }
    
    public make(fixPosition = false) {
        var innerContainer = document.createElement("a");
        $(innerContainer)
            .addClass("favorite-star")
            .attr("href", "#");

        var popoverOptions: PopoverOptions = {
            html: true,
            placement: "right",
            content: () => this.render()
        };
        if (fixPosition) {
            popoverOptions.container = "body";
        }

        var glyphIconType = this.isItemLabeled ? "glyphicon-star" : "glyphicon-star-empty";
        var glyphIcon = this.createGlyphIcon(glyphIconType);
        $(glyphIcon)
            .attr("data-title", "Oblíbené položky")
            .attr("data-toggle", "popover")
            .popover(popoverOptions);
        $(glyphIcon).on("shown.bs.popover", () => {
            this.initPopoverEvents($(glyphIcon));
        });

        innerContainer.appendChild(glyphIcon);
        this.container.append(innerContainer);
    }

    private initPopoverEvents(icon: JQuery) {
        $(".show-all-favorite-button").click(() => {
            icon.popover("hide");

            this.favoriteDialog.show(this.favoriteDefaultTitle);
        });

        $(".fast-add-favorite-label").click((event) => {
            var labelId = $(event.currentTarget).data("id");
            var labelName = $(event.currentTarget).data("name");
            var labelColor = $(event.currentTarget).data("color");
            var favoriteLabel: IFavoriteLabel = {
                Id: labelId,
                Name: labelName,
                Color: labelColor,
                IsDefault: null,
                LastUseTime: null
            };

            this.favoriteManager.createFavoriteItem(this.favoriteItemType, this.itemId, this.favoriteDefaultTitle, labelId, (id) => {
                icon.popover("hide");
                this.popoverBuilder.addFavoriteItem({
                    Id: id,
                    FavoriteType: this.favoriteItemType,
                    Title: this.favoriteDefaultTitle,
                    CreateTime: null,
                    FavoriteLabel: favoriteLabel
                });
            });
        });
    }

    private render() {
        return this.popoverBuilder.getHtmlString();
    }
}

class FavoritePopoverBuilder {
    private templateStart = '<div class="row"><div class="col-md-12"><h6>Seznam přiřazených štítků</h6><div class="favorite-label-popover-container">';
    private templateMiddle = '</div><hr></div></div><div class="row"><div class="col-md-12"><h6>Přidat štítek z naposledy použitých:</h6>';
    private templateEnd = '<hr></div></div><div class="row"><div class="col-md-12"><button type="button" class="btn btn-default btn-block btn-sm show-all-favorite-button">Zobrazit všechny štítky</button></div></div>';

    private favoriteItems: Array<string>;
    private favoriteLabels: Array<string>;

    constructor() {
        this.favoriteItems = [];
        this.favoriteLabels = [];
    }

    public addFavoriteItem(item: IFavoriteBaseInfo) {
        var itemHtml = '<div><span class="label label-favorite" style="background-color: ' + item.FavoriteLabel.Color + '">' + item.FavoriteLabel.Name + '</span><span> ' + item.Title + '</span></div>';
        this.favoriteItems.push(itemHtml);
    }

    public addFavoritLabel(label: IFavoriteLabel) {
        var labelHtml = '<span class="label-favorite-container"><a href="#" class="fast-add-favorite-label" data-id="' + label.Id + '" data-color="' + label.Color + '" + data-name="' + label.Name
            + '"><span class="label label-favorite" style="background-color: ' + label.Color + '">' + label.Name + '</span></a></span>';
        this.favoriteLabels.push(labelHtml);
    }

    public getHtmlString(): string {
        if (this.favoriteItems.length === 0) {
            let emptyInfoHtml = '<div>Žádná položka</div>';
            this.favoriteItems.push(emptyInfoHtml);
        }

        if (this.favoriteLabels.length === 0) {
            let emptyInfoHtml = '<div>Žádná položka</div>';
            this.favoriteLabels.push(emptyInfoHtml);
        }

        return this.templateStart +
            this.favoriteItems.join("") +
            this.templateMiddle +
            this.favoriteLabels.join("") +
            this.templateEnd;
    }
}