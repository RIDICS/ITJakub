class FavoriteQuery {
    private queryType: QueryTypeEnum;
    private bookType: BookTypeEnum;
    private inputTextbox: JQuery;
    private renderContainer: JQuery;
    private favoriteManager: FavoriteManager;
    private favoriteDialog: NewFavoriteDialog;
    private isCreated: boolean;

    constructor(renderContainer: JQuery, inputTextbox: JQuery, bookType: BookTypeEnum, queryType: QueryTypeEnum) {
        this.queryType = queryType;
        this.bookType = bookType;
        this.favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
        this.favoriteDialog = new NewFavoriteDialog(this.favoriteManager);
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
        this.renderContainer.load(url, null, (responseTxt, statusTxt) => {
            if (statusTxt === "success") {
                this.bindEvents();
                this.isCreated = true;
            }
        });

        this.favoriteDialog.make();
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

        $(".favorite-query-item", this.renderContainer).click((event) => {
            var elementJquery = $(event.currentTarget);
            var query = elementJquery.data("query");
            this.inputTextbox.val(query);
        });

        $(".favorite-query-save-button", this.renderContainer).click(() => {
            this.favoriteDialog.show("Nový oblíbený dotaz");
        });

        this.favoriteDialog.setSaveCallback(data => {
            // todo save favorite query
            // this.favoriteManager.createFavoriteItem();

            console.log("Saving favorite query: BT:" + this.bookType + " QT:" + this.queryType);
            throw Error("Not implemented");
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