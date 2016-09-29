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
        this.renderContainer.hide();
    }

    public make() {
        if (this.isCreated) {
            return;
        }

        this.forceRerender();
        this.favoriteDialog.make();
    }

    private forceRerender() {
        this.renderLoading();

        var labels = null;
        var queries = null;
        var labelsLoaded = false;
        var queriesLoaded = false;

        this.favoriteManager.getFavoriteLabels(favoriteLabels => {
            labelsLoaded = true;
            labels = favoriteLabels;

            if (labelsLoaded && queriesLoaded) {
                this.render(labels, queries);
                this.bindEvents();
                this.isCreated = true;
            }
        });
        this.favoriteManager.getFavoriteQueries(this.bookType, this.queryType, favoriteQueries => {
            queriesLoaded = true;
            queries = favoriteQueries;

            if (labelsLoaded && queriesLoaded) {
                this.render(labels, queries);
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

    private render(favoriteLabels: IFavoriteLabel[], favoriteQueries: IFavoriteQuery[]) {
        var mainDiv = document.createElement("div");
        var row1Div = document.createElement("div");
        var filterColumnDiv = document.createElement("div");
        var filterHeadingSpan = document.createElement("span");
        var filterSeparator = document.createElement("hr");
        var filterContainer = document.createElement("div");
        var displayAllLink = document.createElement("a");
        
        var listColumnDiv = document.createElement("div");
        var listHeadingSpan = document.createElement("span");
        var listHeadingLabel = document.createElement("span");
        var listSeparator = document.createElement("hr");
        var listContainer = document.createElement("div");

        $(filterHeadingSpan).text("Filtrovat:");

        $(displayAllLink)
            .attr("href", "#")
            .addClass("favorite-query-label")
            .data("id", 0)
            .data("name", "Zobrazeno vše")
            .data("color", "#0000DD")
            .text("Zobrazit vše");

        $(filterContainer)
            .addClass("favorite-query-list")
            .append(displayAllLink);

        $(filterColumnDiv)
            .addClass("col-md-3")
            .append(filterHeadingSpan)
            .append(filterSeparator)
            .append(filterContainer);

        for (let i = 0; i < favoriteLabels.length; i++) {
            var favoriteLabel = favoriteLabels[i];
            var labelLink = document.createElement("a");
            var label = document.createElement("span");

            let color = new HexColor(favoriteLabel.Color);
            let fontColor = FavoriteHelper.getDefaultFontColor(color);
            let borderColor = FavoriteHelper.getDefaultBorderColor(color);

            $(label)
                .addClass("label")
                .css("background-color", favoriteLabel.Color)
                .css("border-color", borderColor)
                .css("color", fontColor)
                .text(favoriteLabel.Name);

            $(labelLink)
                .attr("href", "#")
                .addClass("favorite-query-label")
                .data("id", favoriteLabel.Id)
                .data("name", favoriteLabel.Name)
                .data("color", favoriteLabel.Color)
                .append(label);

            $(filterContainer).append(labelLink);
        }

        $(listHeadingSpan)
            .text("Vložit dotaz z oblíbených: ");
        $(listHeadingLabel)
            .addClass("label")
            .addClass("favorite-query-label-selected")
            .css("background-color", "#0000DD")
            .text("Zobrazeno vše");

        $(listContainer)
            .addClass("favorite-query-list");

        $(listColumnDiv)
            .addClass("col-md-9")
            .append(listHeadingSpan)
            .append(listHeadingLabel)
            .append(listSeparator)
            .append(listContainer);

        for (let i = 0; i < favoriteQueries.length; i++) {
            var favoriteQuery = favoriteQueries[i];
            var queryLink = document.createElement("a");
            var queryRow1 = document.createElement("div");
            var queryRow2 = document.createElement("div");
            var queryLabel = document.createElement("span");
            var queryTitle = document.createElement("span");
            var queryRemoveLink = document.createElement("a");
            var queryRemoveIcon = document.createElement("span");

            $(queryRemoveIcon)
                .addClass("glyphicon")
                .addClass("glyphicon-trash");

            $(queryRemoveLink)
                .attr("href", "#")
                .addClass("favorite-query-remove")
                .data("id", favoriteQuery.Id)
                .append(queryRemoveIcon);

            let color = new HexColor(favoriteQuery.FavoriteLabel.Color);
            let fontColor = FavoriteHelper.getDefaultFontColor(color);
            let borderColor = FavoriteHelper.getDefaultBorderColor(color);
            
            $(queryLabel)
                .addClass("label")
                .css("background-color", favoriteQuery.FavoriteLabel.Color)
                .css("border-color", borderColor)
                .css("color", fontColor)
                .text(favoriteQuery.FavoriteLabel.Name);

            $(queryTitle)
                .text(" " + favoriteQuery.Title);

            $(queryRow1)
                .append(queryLabel)
                .append(queryTitle);

            $(queryRow2)
                .addClass("favorite-query-raw")
                .text(favoriteQuery.Query);

            $(queryLink)
                .attr("href", "#")
                .addClass("favorite-query-item")
                .data("id", favoriteQuery.Id)
                .data("query", favoriteQuery.Query)
                .data("label-id", favoriteQuery.FavoriteLabel.Id)
                .append(queryRemoveLink)
                .append(queryRow1)
                .append(queryRow2);

            $(listContainer).append(queryLink);
        }
        if (favoriteQueries.length === 0) {
            var noQueryDiv = document.createElement("div");
            $(noQueryDiv)
                .css("margin-left", "15px")
                .text("Žádný oblíbený dotaz");
            $(listContainer).append(noQueryDiv);
        }

        $(row1Div)
            .addClass("row")
            .append(filterColumnDiv)
            .append(listColumnDiv);

        var row2Div = document.createElement("div");
        var separatorDiv = document.createElement("div");
        var separator = document.createElement("hr");

        $(separatorDiv)
            .addClass("col-md-12")
            .append(separator);

        $(row2Div)
            .addClass("row")
            .append(separatorDiv);

        
        var row3Div = document.createElement("div");
        var row3InnerDiv = document.createElement("div");
        var saveButton = document.createElement("button");
        var buttonIcon = document.createElement("span");
        var buttonText = document.createElement("span");

        $(buttonIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-star-empty");
        $(buttonText)
            .text(" Uložit dotaz jako oblíbený");

        $(saveButton)
            .addClass("btn")
            .addClass("btn-default")
            .addClass("btn-block")
            .addClass("favorite-query-save-button")
            .append(buttonIcon)
            .append(buttonText);

        $(row3InnerDiv)
            .addClass("col-md-5")
            .addClass("col-md-offset-7")
            .append(saveButton);

        $(row3Div)
            .addClass("row")
            .append(row3InnerDiv);

        $(mainDiv)
            .addClass("favorite-query")
            .append(row1Div)
            .append(row2Div)
            .append(row3Div);

        this.renderContainer.empty();
        this.renderContainer.append(mainDiv);
    }

    public isHidden(): boolean {
        return this.renderContainer.is(":hidden");
    }

    public hide() {
        //this.renderContainer.hide();
        this.renderContainer.slideUp();
    }

    public show() {
        if (!this.isCreated) {
            this.make();
        }
        //this.renderContainer.show();
        this.renderContainer.slideDown();
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

        $(".favorite-query-remove", this.renderContainer).click((event) => {
            event.stopPropagation();

            var elementJQuery = $(event.currentTarget);
            var favoriteId = elementJQuery.data("id");
            this.favoriteManager.deleteFavoriteItem(favoriteId, () => {
                elementJQuery.closest(".favorite-query-item").remove();
            });
        });

        this.favoriteDialog.setSaveCallback(data => {
            this.saveFavoriteQuery(data.itemName, data.labelId);
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

    private saveFavoriteQuery(itemName: string, labelId: number) {
        var query = this.inputTextbox.val();
        this.favoriteManager.createFavoriteQuery(this.bookType, this.queryType, query, itemName, labelId, (id, error) => {
            if (error) {
                this.favoriteDialog.showError("Chyba při vytváření oblíbeného dotazu");
                return;
            }

            this.forceRerender();
            this.favoriteDialog.hide();
        });
    }
}