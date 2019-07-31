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

    //private localization : Localization;
	private localizationScope = "FavoriteJs";

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

        //this.localization = new Localization();
    }

    private createGlyphIcon(name: string): HTMLSpanElement {
        var span = document.createElement("span");
        $(span).addClass("glyphicon")
            .addClass(name);

        return span;
    }

    public addFavoriteItems(items: Array<IFavoriteBaseInfoWithLabel>) {
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
    
    public make(placement: BootstrapPlacement = "right", fixPosition = false) {
        var innerContainer = document.createElement("a");
        $(innerContainer)
            .addClass("favorite-star")
            .attr("href", "#");

        var popoverOptions: PopoverOptions = {
            html: true,
            placement: placement ? placement : "right",
            content: () => this.renderPopover()
        };
        if (fixPosition) {
            popoverOptions.container = "body";
        }

        var glyphIconType = this.isItemLabeled ? "glyphicon-star" : "glyphicon-star-empty";
        var glyphIcon = this.createGlyphIcon(glyphIconType);
        $(glyphIcon)
            .attr("data-title", localization.translate("ThisItemLabels", this.localizationScope).value)
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
            var labelId = $(event.currentTarget as Node as Element).data("id");
            var labelName = $(event.currentTarget as Node as Element).data("name");
            var labelColor = $(event.currentTarget as Node as Element).data("color");
            
            this.createFavoriteItemFast(labelId, this.favoriteDefaultTitle, labelName, labelColor);
        });

        $(".favorite-book-remove").click((event) => {
            var elementJQuery = $(event.currentTarget as Node as HTMLElement);
            var id = <number>elementJQuery.data("id");

            this.deleteFavoriteItem(id, elementJQuery);
        });

        $("[data-toggle=tooltip]").tooltip();
    }

    private createFavoriteItemObject(id: number, favoriteTitle: string, labelId: number, labelName: string, labelColor: string): IFavoriteBaseInfoWithLabel {
        var favoriteLabel: IFavoriteLabel = {
            id: labelId,
            name: labelName,
            color: labelColor,
            isDefault: null,
            lastUseTime: null
        };
        var favoriteItem: IFavoriteBaseInfoWithLabel = {
            id: id,
            favoriteType: this.favoriteItemType,
            title: favoriteTitle,
            createTime: null,
            favoriteLabel: favoriteLabel
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
                this.favoriteDialog.showError(localization.translate("CreateFavItemError", this.localizationScope).value);
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
        if (favoriteTitle.length > FavoriteManager.maxTitleLength) {
            const ellipsis = "...";
            favoriteTitle = favoriteTitle.substr(0, FavoriteManager.maxTitleLength - ellipsis.length) + ellipsis;
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
                itemsContainerJQuery.text(localization.translate("NoItem", this.localizationScope).value);
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
    private localizationScope = "FavoriteJs";

    private templateStart: string;
    private templateMiddle: string;
    private templateEnd: string;
    private favoriteItems: Array<IFavoriteBaseInfoWithLabel>;
    private favoriteLabels: Array<IFavoriteLabel>;

    constructor() {
        this.favoriteItems = [];
        this.favoriteLabels = [];

        var attachedTagList = localization.translate("AttachedTagList", this.localizationScope).value;
        var addTagFromLastUsed = localization.translate("AddTagFromLastUsed", this.localizationScope).value;
        var advancedOptions = localization.translate("AdvancedOptions", this.localizationScope).value;

        this.templateStart = `<div class="row"><div class="col-md-12"><h6>${attachedTagList}</h6><div class="favorite-label-popover-container">`;
        this.templateMiddle = `</div><hr></div></div><div class="row"><div class="col-md-12"><h6>${addTagFromLastUsed}</h6>`;
        this.templateEnd = `<hr></div></div><div class="row"><div class="col-md-12"><button type="button" class="btn btn-default btn-block btn-sm show-all-favorite-button">${advancedOptions}</button></div></div>`;
    }

    private getFavoriteItemHtml(item: IFavoriteBaseInfoWithLabel): string {
        var color = new HexColor(item.favoriteLabel.color);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);
        var title = localization.translate("SavedAs", this.localizationScope).value;

        return `<div class="favorite-item"><span class="label label-favorite" data-toggle="tooltip" title="${title}${item.title
            }" style="background-color: ${escapeHtmlChars(item.favoriteLabel.color)}; border-color:${borderColor}; color: ${fontColor};">${escapeHtmlChars(item.favoriteLabel.name)
            }<a href="#" class="favorite-book-remove" data-id="${escapeHtmlChars(item.id.toString())
            }" style="color: ${fontColor}"><span class="glyphicon glyphicon-remove"></span></a></span></div>`;
    }

    private getFavoriteLabelHtml(label: IFavoriteLabel): string {
        var color = new HexColor(label.color);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);
        return `<span class="label-favorite-container"><a href="#" class="fast-add-favorite-label" data-id="${escapeHtmlChars(label.id.toString())
            }" data-color="${escapeHtmlChars(label.color)}" + data-name="${escapeHtmlChars(label.name)
            }"><span class="label label-favorite" style="background-color: ${escapeHtmlChars(label.color)}; border-color: ${borderColor}; color: ${fontColor};">${escapeHtmlChars(label.name)
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

        var title = localization.translate("AddFromAllTagList", this.localizationScope).value;

        var nextButtonString = `<span class="label-favorite-container"><a href="#" class="show-all-favorite-button" title="${title}"><span style="color: black; font-weight: bold; margin-left: 3px;">...</span></a></span>`;
        resultStrings.push(nextButtonString);

        return resultStrings.join("");
    }

    public getFavoriteItemsCount(): number {
        return this.favoriteItems.length;
    }

    public addFavoriteItem(item: IFavoriteBaseInfoWithLabel) {
        
        this.favoriteItems.push(item);
    }

    public addFavoritLabel(label: IFavoriteLabel) {
        
        this.favoriteLabels.push(label);
    }

    public removeFavoriteItem(id: number) {
        for (var i = 0; i < this.favoriteItems.length; i++) {
            var favoriteItem = this.favoriteItems[i];
            if (favoriteItem.id === id) {
                this.favoriteItems.splice(i, 1);
                return;
            }
        }
    }

    public getHtmlString(): string {
        var favoriteItemsString = this.favoriteItems.length === 0
            ? "<div>" + localization.translate("NoItem", this.localizationScope).value + "</div>"
            : this.getFavoriteItemsHtml();
        var favoriteLabelsString = this.favoriteLabels.length === 0
            ? "<div>" + localization.translate("NoItem", this.localizationScope).value + "</div>"
            : this.getFavoriteLabelsHtml();

        return this.templateStart +
            favoriteItemsString +
            this.templateMiddle +
            favoriteLabelsString +
            this.templateEnd;
    }
}

interface IFavoriteChangedInfo {
    addedLabelsName: string;
    addedLabels: Array<IFavoriteLabel>;
    removedLabelIds: Array<number>;
}