class FavoriteQuery {
    private inputTextbox: JQuery;
    private renderContainer: JQuery;
    private isCreated: boolean;

    constructor(renderContainer: JQuery, inputTextbox: JQuery) {
        this.inputTextbox = inputTextbox;
        this.renderContainer = renderContainer;
        this.isCreated = false;
        this.hide();
    }

    public make() {
        if (this.isCreated) {
            return;
        }

        var url = getBaseUrl() + "Favorite/GetFavoriteQueryPartial";
        this.renderContainer.text("Načítání");
        this.renderContainer.load(url);
    }

    public isHidden(): boolean {
        return this.renderContainer.is(":hidden");
    }

    public hide() {
        this.renderContainer.hide();
    }

    public show() {
        if (!this.isCreated) {
            this.make();
        }
        this.renderContainer.show();
    }
}