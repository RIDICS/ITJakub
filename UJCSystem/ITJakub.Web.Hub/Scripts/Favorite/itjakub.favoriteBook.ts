class FavoriteBook {
    private dropdownSelect: DropDownSelect2;
    private bookType: BookTypeEnum;
    private container: JQuery;
    private bodyDiv: HTMLDivElement;
    private favoriteManager: FavoriteManager;
    private loading: boolean;

    constructor(container: JQuery, bookType: BookTypeEnum, dropdownSelect: DropDownSelect2) {
        this.dropdownSelect = dropdownSelect;
        this.bookType = bookType;
        this.container = container;
        this.favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
        this.loading = false;
    }

    public make() {
        var innerContainer = document.createElement("div");
        var headerDiv = document.createElement("div");
        var headerButton = document.createElement("button");
        var favoriteIcon = document.createElement("span");
        var buttonContentSpan = document.createElement("span");
        $(favoriteIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-star-empty");
        $(buttonContentSpan)
            .text(" Vybrat oblíbené knihy a kategorie");
        $(headerButton)
            .attr("type", "button")
            .addClass("favorite-book-select-button")
            .append(favoriteIcon)
            .append(buttonContentSpan)
            .click(this.toggleBodyVisibility.bind(this));

        $(headerDiv)
            .addClass("dropdown-select-header")
            .append(headerButton);

        this.bodyDiv = document.createElement("div");
        
        $(this.bodyDiv)
            .addClass("dropdown-select-body")
            .addClass("favorite-book-select-body");

        $(innerContainer)
            .addClass("dropdown-select")
            .addClass("favorite-book-select")
            .append(headerDiv)
            .append(this.bodyDiv);

        this.container.append(innerContainer);
        this.loadData();

        $(document).unbind("click.favoriteBooks");
        $(document).bind("click.favoriteBooks", (event) => {
            var parents = $(event.target).parents();
            if (!parents.is(this.container)) {
                this.hide();
            }
        });
    }

    private toggleBodyVisibility() {
        var bodyJQuery = $(this.bodyDiv);
        if (bodyJQuery.is(":hidden")) {
            this.show();
        } else {
            this.hide();
        }
    }

    public show() {
        $(this.bodyDiv).slideDown("fast");
    }

    public hide() {
        $(this.bodyDiv).slideUp("fast");
    }

    public loadData() {
        if (this.loading) {
            return;
        }

        this.loading = true;
        $(this.bodyDiv)
            .addClass("loading")
            .empty();

        this.favoriteManager.getFavoriteLabelsForBooksAndCategories(this.bookType, (favoriteLabels) => {
            $(this.bodyDiv).removeClass("loading");

            for (var i = 0; i < favoriteLabels.length; i++) {
                var favoriteLabel = favoriteLabels[i];
                var itemDiv = this.createFavoriteLabel(favoriteLabel);
                this.bodyDiv.appendChild(itemDiv);
            }

            if (favoriteLabels.length === 0) {
                var emptyDiv = document.createElement("div");
                $(emptyDiv)
                    .addClass("text-center")
                    .text("Žádný štítek neobsahuje oblíbenou knihu nebo kategorii");
                this.bodyDiv.appendChild(emptyDiv);
            }

            this.loading = false;
        });
    }

    private createFavoriteLabel(favoriteLabel: IFavoriteLabelsWithBooksAndCategories): HTMLDivElement {
        var mainDiv = document.createElement("div");
        var link = document.createElement("a");
        var labelSpan = document.createElement("span");

        var color = new HexColor(favoriteLabel.Color);
        var fontColor = FavoriteHelper.getDefaultFontColor(color);
        var borderColor = FavoriteHelper.getDefaultBorderColor(color);

        $(labelSpan)
            .addClass("label")
            .addClass("favorite-dropdown-item-label")
            .text(favoriteLabel.Name)
            .css("background-color", favoriteLabel.Color)
            .css("border-color", borderColor)
            .css("color", fontColor);

        $(link)
            .attr("href", "#")
            .data("id", favoriteLabel.Id)
            .data("bookIdList", favoriteLabel.BookIdList)
            .data("categoryIdList", favoriteLabel.CategoryIdList)
            .click(this.onLabelClick.bind(this))
            .append(labelSpan);

        $(mainDiv)
            .append(link);

        return mainDiv;
    }

    private onLabelClick(event: JQueryEventObject) {
        var linkJquery = $(event.currentTarget);
        var bookIdList = <number[]>linkJquery.data("bookIdList");
        var categoryIdList = <number[]>linkJquery.data("categoryIdList");

        this.dropdownSelect.setSelected(categoryIdList, bookIdList);
        setTimeout(() => this.dropdownSelect.showBody(), 0);
    }
}