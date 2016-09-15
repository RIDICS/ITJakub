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
        this.renderContainer.load(url, null, (responseTxt, statusTxt, xhr) => {
            if (statusTxt === "success") {
                this.bindEvents();
                this.isCreated = true;
            }
        });
    }

    private renderLoading() {
        var innerContainerDiv = document.createElement("div");
        $(innerContainerDiv).addClass("favorite-query");

        var loadingDiv = document.createElement("div");
        $(loadingDiv).addClass("loading");

        $(innerContainerDiv).append(loadingDiv);
        
        this.renderContainer.empty();
        this.renderContainer.append(innerContainerDiv);
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

    private bindEvents() {
        $(".favorite-query-label", this.renderContainer).click((event) => {
            var elementJquery = $(event.currentTarget);
            var labelId = elementJquery.data("id");
            var labelName = elementJquery.data("name");
            var labelColor = elementJquery.data("color");

            $(".favorite-query-label-selected")
                .data("id", labelId)
                .text(labelName)
                .css("background-color", labelColor);

            this.filterQueryList(labelId);
        });
    }

    private filterQueryList(labelId?: number) {
        if (!labelId) {
            $(".favorite-query-item").removeClass("hidden");
        } else {
            $(".favorite-query-item")
                .addClass("hidden")
                .each((index, element) => {
                    var elementLabelId = $(element).data("label-id");
                    if (elementLabelId === labelId) {
                        $(element).removeClass("hidden");
                    }
                });
        }
    }
}