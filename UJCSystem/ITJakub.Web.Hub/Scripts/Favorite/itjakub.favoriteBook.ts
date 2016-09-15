class FavoriteBook {
    private container: JQuery;
    private bodyDiv: HTMLDivElement;
    private favoriteManager: FavoriteManager;

    constructor(container: JQuery) {
        this.container = container;
        this.favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
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
            .text("Vybrat oblíbené / DropdownSelect");
        $(headerButton)
            .attr("style", "width: 100%; height: 100%;")
            .attr("type", "button")
            .append(favoriteIcon)
            .append(buttonContentSpan)
            .click(this.toggleBodyVisibility.bind(this));

        $(headerDiv)
            .addClass("dropdown-select-header")
            .append(headerButton);

        this.bodyDiv = document.createElement("div");
        $(this.bodyDiv)
            .addClass("dropdown-select-body")
            .text("TODO tag selection");

        $(innerContainer)
            .addClass("dropdown-select")
            .append(headerDiv)
            .append(this.bodyDiv);

        this.container.append(innerContainer);
        this.loadData();
    }

    private toggleBodyVisibility() {
        var bodyJQuery = $(this.bodyDiv);

        if (bodyJQuery.is(":hidden")) {
            bodyJQuery.show();
        } else {
            bodyJQuery.hide();
        }
    }

    private loadData() {
        //this.favoriteManager.getFavoriteLabelsForBooksAndCategories((favoriteLabels) => {
            
        //});
    }

    private createFavoriteLabel(favoriteLabel: IFavoriteLabel): HTMLDivElement {
        var mainDiv = document.createElement("div");
        var link = document.createElement("a");
        var labelSpan = document.createElement("span");

        $(labelSpan)
            .addClass("label")
            .text(favoriteLabel.Name)
            .css("background-color", favoriteLabel.Color);

        $(link)
            .attr("href", "#")
            .append(labelSpan);

        $(mainDiv)
            .append(link);

        return mainDiv;
    }
}