class FavoriteStar {
    private favoritesChangedCallback: () => void;
    private favoriteManager: FavoriteManager;
    private favoriteDialog: NewFavoriteDialog;
    private favoriteDefaultTitle: string;
    private favoriteItemType: FavoriteType;
    private itemId: string;
    private container: JQuery;
    private isItemLabeled: boolean;
    private popoverBuilder: FavoritePopoverBuilder;
    private starGlyphIcon: HTMLSpanElement;

    constructor(container: JQuery, type: FavoriteType, itemId: string, favoriteDefaultTitle: string, favoriteDialog: NewFavoriteDialog, favoriteManager: FavoriteManager, favoritesChangedCallback: () => void) {
        this.favoritesChangedCallback = favoritesChangedCallback;
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
            content: () => this.renderPopover()
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
            this.initPopoverEvents();
        });
        this.starGlyphIcon = glyphIcon;

        innerContainer.appendChild(glyphIcon);
        this.container.append(innerContainer);
    }

    private initPopoverEvents() {
        $(".show-all-favorite-button").click(() => {
            $(this.starGlyphIcon).popover("hide");

            this.favoriteDialog.setSaveCallback(data => this.createFavoriteItem(data.labelId, data.itemName, data.labelName, data.labelColor));
            this.favoriteDialog.show(this.favoriteDefaultTitle);
        });

        $(".fast-add-favorite-label").click((event) => {
            var labelId = $(event.currentTarget).data("id");
            var labelName = $(event.currentTarget).data("name");
            var labelColor = $(event.currentTarget).data("color");
            
            this.createFavoriteItem(labelId, this.favoriteDefaultTitle, labelName, labelColor);
        });

        $(".favorite-book-remove").click((event) => {
            var elementJQuery = $(event.currentTarget);
            var id = <number>elementJQuery.data("id");

            this.deleteFavoriteItem(id, elementJQuery);
        });
    }

    private createFavoriteItem(labelId: number, favoriteTitle: string, labelName: string, labelColor: string) {
        this.favoriteManager.createFavoriteItem(this.favoriteItemType, this.itemId, favoriteTitle, labelId, (id) => {
            $(this.starGlyphIcon).popover("hide");

            var favoriteLabel: IFavoriteLabel = {
                Id: labelId,
                Name: labelName,
                Color: labelColor,
                IsDefault: null,
                LastUseTime: null
            };
            var favoriteItem: IFavoriteBaseInfo = {
                Id: id,
                FavoriteType: this.favoriteItemType,
                Title: favoriteTitle,
                CreateTime: null,
                FavoriteLabel: favoriteLabel
            };

            this.popoverBuilder.addFavoriteItem(favoriteItem);
            this.notifyFavoritesChanged();
        });
    }

    private deleteFavoriteItem(id: number, elementJQuery: JQuery) {
        this.favoriteManager.deleteFavoriteItem(id, () => {
            var itemsContainerJQuery = elementJQuery.closest(".favorite-label-popover-container");

            this.popoverBuilder.removeFavoriteItem(id);
            elementJQuery.closest(".favorite-item").remove();
            this.notifyFavoritesChanged();

            if (this.popoverBuilder.getFavoriteItemsCount() === 0) {
                itemsContainerJQuery.text("Žádná položka");
            }
        });
    }

    private notifyFavoritesChanged() {
        this.updateStarState();

        if (this.favoritesChangedCallback) {
            this.favoritesChangedCallback();
        }
    }

    private updateStarState() {
        if (this.popoverBuilder.getFavoriteItemsCount() === 0) {
            $(this.starGlyphIcon)
                .removeClass("glyphicon-star")
                .addClass("glyphicon-star-empty");
        } else {
            $(this.starGlyphIcon)
                .removeClass("glyphicon-star-empty")
                .addClass("glyphicon-star");
        }
    }

    private renderPopover() {
        return this.popoverBuilder.getHtmlString();
    }
}

class FavoritePopoverBuilder {
    private templateStart = '<div class="row"><div class="col-md-12"><h6>Seznam přiřazených štítků</h6><div class="favorite-label-popover-container">';
    private templateMiddle = '</div><hr></div></div><div class="row"><div class="col-md-12"><h6>Přidat štítek z naposledy použitých:</h6>';
    private templateEnd = '<hr></div></div><div class="row"><div class="col-md-12"><button type="button" class="btn btn-default btn-block btn-sm show-all-favorite-button">Zobrazit všechny štítky</button></div></div>';

    private favoriteItems: Array<IFavoriteBaseInfo>;
    private favoriteLabels: Array<IFavoriteLabel>;

    constructor() {
        this.favoriteItems = [];
        this.favoriteLabels = [];
    }

    private getFavoriteItemHtml(item: IFavoriteBaseInfo): string {
        return '<div class="favorite-item"><a href="#" class="favorite-book-remove" data-id="' + item.Id
            + '"><span class="glyphicon glyphicon-trash"></span></a><span class="label label-favorite" style="background-color: ' + item.FavoriteLabel.Color + '">'
            + item.FavoriteLabel.Name + '</span><span> ' + item.Title + '</span></div>';
    }

    private getFavoriteLabelHtml(label: IFavoriteLabel): string {
        return '<span class="label-favorite-container"><a href="#" class="fast-add-favorite-label" data-id="' + label.Id + '" data-color="' + label.Color + '" + data-name="' + label.Name
            + '"><span class="label label-favorite" style="background-color: ' + label.Color + '">' + label.Name + '</span></a></span>';
    }

    private getFavoriteItemsHtml(): string {
        var resultStrings = new Array<string>();
        for (var i = 0; i < this.favoriteItems.length; i++) {
            var itemHtml = this.getFavoriteItemHtml(this.favoriteItems[i]);
            resultStrings.push(itemHtml);
        }
        return resultStrings.join("");
    }

    private getFavoriteLabelsHtml(): string {
        var resultStrings = new Array<string>();
        for (var i = 0; i < this.favoriteLabels.length; i++) {
            var labelHtml = this.getFavoriteLabelHtml(this.favoriteLabels[i]);
            resultStrings.push(labelHtml);
        }
        return resultStrings.join("");
    }

    public getFavoriteItemsCount(): number {
        return this.favoriteItems.length;
    }

    public addFavoriteItem(item: IFavoriteBaseInfo) {
        
        this.favoriteItems.push(item);
    }

    public addFavoritLabel(label: IFavoriteLabel) {
        
        this.favoriteLabels.push(label);
    }

    public removeFavoriteItem(id: number) {
        for (var i = 0; i < this.favoriteItems.length; i++) {
            var favoriteItem = this.favoriteItems[i];
            if (favoriteItem.Id === id) {
                this.favoriteItems.splice(i, 1);
                return;
            }
        }
    }

    public getHtmlString(): string {
        var favoriteItemsString = this.favoriteItems.length === 0
            ? "<div>Žádná položka</div>"
            : this.getFavoriteItemsHtml();
        var favoriteLabelsString = this.favoriteLabels.length === 0
            ? "<div>Žádná položka</div>"
            : this.getFavoriteLabelsHtml();

        return this.templateStart +
            favoriteItemsString +
            this.templateMiddle +
            favoriteLabelsString +
            this.templateEnd;
    }
}