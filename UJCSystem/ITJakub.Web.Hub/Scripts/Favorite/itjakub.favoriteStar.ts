class FavoriteStar {
    private container: JQuery;
    private popoverBuilder: FavoritePopoverBuilder;

    constructor(container: JQuery) {
        this.container = container;
        this.popoverBuilder = new FavoritePopoverBuilder();
    }

    private createGlyphIcon(name: string): HTMLSpanElement {
        var span = document.createElement("span");
        $(span).addClass("glyphicon")
            .addClass(name);

        return span;
    }

    public addFavoriteItems(items: Array<IFavoriteItem>) {
        for (var i = 0; i < items.length; i++) {
            this.popoverBuilder.addFavoriteItem(items[i]);
        }
    }

    public addFavoriteLabels(labels: Array<IFavoriteLabel>) {
        for (var i = 0; i < labels.length; i++) {
            this.popoverBuilder.addFavoritLabel(labels[i]);
        }
    }
    
    public make() {
        var innerContainer = document.createElement("div");
        $(innerContainer).addClass("favorite-star");

        var glyphIcon = this.createGlyphIcon("glyphicon-star-empty");
        $(glyphIcon)
            .attr("data-content", this.popoverBuilder.getHtmlString())
            .attr("data-title", "Oblíbené položky")
            .attr("data-toggle", "popover")
            .popover({
                html: true,
                placement: "right"
            });

        innerContainer.appendChild(glyphIcon);
        this.container.append(innerContainer);
    }
}

class FavoritePopoverBuilder {
    private templateStart = '<div class="row"><div class="col-md-6"><h6>Existující</h6>';
    private templateMiddle = '</div><div class="col-md-6"><h6>Rychlé přidání</h6>';
    private templateEnd = '</div></div><div class="row"><div class="col-md-12"><button type="button" class="btn btn-default btn-block">Zobrazit všechny štítky</button></div></div>';

    private favoriteItems: Array<string>;
    private favoriteLabels: Array<string>;

    constructor() {
        this.favoriteItems = [];
        this.favoriteLabels = [];
    }

    public addFavoriteItem(item: IFavoriteItem) {
        var itemHtml = '<div><span class="badge" style="background-color: ' + item.FavoriteLabel.Color + '">' + item.FavoriteLabel.Name + '</span><span> ' + item.Title + '</span></div>';
        this.favoriteItems.push(itemHtml);
    }

    public addFavoritLabel(label: IFavoriteLabel) {
        var labelHtml = '<div><a href="#" data-id="' + label.Id + '"><span class="badge" style="background-color: ' + label.Color + '">' + label.Name + '</span></a></div>';
        this.favoriteLabels.push(labelHtml);
    }

    public getHtmlString(): string {
        if (this.favoriteItems.length === 0) {
            var emptyInfoHtml = '<div>Žádná položka</div>';
            this.favoriteItems.push(emptyInfoHtml);
        }

        if (this.favoriteLabels.length === 0) {
            var emptyInfoHtml = '<div>Žádná položka</div>';
            this.favoriteLabels.push(emptyInfoHtml);
        }

        return this.templateStart +
            this.favoriteItems.join("") +
            this.templateMiddle +
            this.favoriteLabels.join("") +
            this.templateEnd;
    }
}