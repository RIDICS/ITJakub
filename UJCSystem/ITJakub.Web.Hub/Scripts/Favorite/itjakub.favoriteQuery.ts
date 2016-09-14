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
        this.renderLoading();
        this.renderContainer.load(url);
    }

    private renderLoading() {
        var loadingDiv = document.createElement("div");
        $(loadingDiv).addClass("loading");
        $(loadingDiv).css("min-height", "100px"); // TODO use css styles

        this.renderContainer.append(loadingDiv);
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