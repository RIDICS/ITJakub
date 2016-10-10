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
            .attr("data-title", "Štítky této položky")
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

            this.favoriteDialog.setSaveCallback(this.createFavoriteItem.bind(this));
            this.favoriteDialog.show(this.favoriteDefaultTitle);
        });

        $(".fast-add-favorite-label").click((event) => {
            var labelId = $(event.currentTarget).data("id");
            var labelName = $(event.currentTarget).data("name");
            var labelColor = $(event.currentTarget).data("color");
            
            this.createFavoriteItemFast(labelId, this.favoriteDefaultTitle, labelName, labelColor);
        });

        $(".favorite-book-remove").click((event) => {
            var elementJQuery = $(event.currentTarget);
            var id = <number>elementJQuery.data("id");

            this.deleteFavoriteItem(id, elementJQuery);
        });

        $("[data-toggle=tooltip]").tooltip();
    }

    private createFavoriteItemObject(id: number, favoriteTitle: string, labelId: number, labelName: string, labelColor: string): IFavoriteBaseInfo {
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
        return favoriteItem;
    }

    private createFavoriteItem(data: INewFavoriteItemData) {
        var labelIds = new Array<number>();
        for (let i = 0; i < data.labels.length; i++) {
            var id = data.labels[i].labelId;
            labelIds.push(id);
        }

        this.favoriteManager.createFavoriteItem(this.favoriteItemType, this.itemId, data.itemName, labelIds, (ids, error) => {
            if (error) {
                this.favoriteDialog.showError("Chyba při vytváření oblíbené položky");
                return;
            }

            this.favoriteDialog.hide();

            for (let i = 0; i < ids.length; i++) {
                var labelData = data.labels[i];
                var itemData = this.createFavoriteItemObject(ids[i], data.itemName, labelData.labelId, labelData.labelName, labelData.labelColor);
                this.popoverBuilder.addFavoriteItem(itemData);
            }
            this.notifyFavoritesChanged();
        });
    }

    private createFavoriteItemFast(labelId: number, favoriteTitle: string, labelName: string, labelColor: string) {
        if (favoriteTitle.length > 250) {
            favoriteTitle = favoriteTitle.substr(0, 247) + "...";
        }
        this.favoriteManager.createFavoriteItem(this.favoriteItemType, this.itemId, favoriteTitle, [labelId], (ids, error) => {
            if (error) {
                return;
            }

            $(this.starGlyphIcon).popover("hide");

            var itemData = this.createFavoriteItemObject(ids[0], favoriteTitle, labelId, labelName, labelColor);
            this.popoverBuilder.addFavoriteItem(itemData);
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
    private templateStart = '<div class="row"><div class="col-md-12"><h6>Seznam přiřazených štítků:</h6><div class="favorite-label-popover-container">';
    private templateMiddle = '</div><hr></div></div><div class="row"><div class="col-md-12"><h6>Přidat štítek z naposledy použitých:</h6>';
    private templateEnd = '<hr></div></div><div class="row"><div class="col-md-12"><button type="button" class="btn btn-default btn-block btn-sm show-all-favorite-button">Pokročilé možnosti</button></div></div>';

    private favoriteItems: Array<IFavoriteBaseInfo>;
    private favoriteLabels: Array<IFavoriteLabel>;

    constructor() {
        this.favoriteItems = [];
        this.favoriteLabels = [];
    }

    private getFavoriteItemHtml(item: IFavoriteBaseInfo): string {
        var color = new HexColor(item.FavoriteLabel.Color);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);
        return `<div class="favorite-item"><span class="label label-favorite" data-toggle="tooltip" title="Uloženo jako: ${item.Title
            }" style="background-color: ${escapeHtmlChars(item.FavoriteLabel.Color)}; border-color:${borderColor}; color: ${fontColor};">${escapeHtmlChars(item.FavoriteLabel.Name)
            }<a href="#" class="favorite-book-remove" data-id="${escapeHtmlChars(item.Id.toString())
            }" style="color: ${fontColor}"><span class="glyphicon glyphicon-remove"></span></a></span></div>`;
    }

    private getFavoriteLabelHtml(label: IFavoriteLabel): string {
        var color = new HexColor(label.Color);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);
        return `<span class="label-favorite-container"><a href="#" class="fast-add-favorite-label" data-id="${escapeHtmlChars(label.Id.toString())
            }" data-color="${escapeHtmlChars(label.Color)}" + data-name="${escapeHtmlChars(label.Name)
            }"><span class="label label-favorite" style="background-color: ${escapeHtmlChars(label.Color)}; border-color: ${borderColor}; color: ${fontColor};">${escapeHtmlChars(label.Name)
            }</span></a></span>`;
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

        var nextButtonString = '<span class="label-favorite-container"><a href="#" class="show-all-favorite-button" title="Přidat ze seznamu všech štítků"><span style="color: black; font-weight: bold; margin-left: 3px;">...</span></a></span>';
        resultStrings.push(nextButtonString);

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