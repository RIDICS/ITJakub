class FavoriteQuery {
    private static pageSize = 20;
    private queryType: QueryTypeEnum;
    private bookType: BookTypeEnum;
    private inputTextbox: JQuery;
    private renderContainer: JQuery;
    private favoriteManager: FavoriteManager;
    private favoriteDialog: NewFavoriteDialog;
    private isCreated: boolean;
    private labelContainer: HTMLDivElement;
    private listContainer: HTMLDivElement;
    private noQueryDiv: HTMLDivElement;
    private insertDialog: InsertQueryDialog;
    private filterQuerySearchBox: FilterSearchBox;
    private filterLabelInput: HTMLInputElement;
    private noFilteredLabel: HTMLDivElement;
    private displayAllLink: HTMLAnchorElement;
    private noSelectedLabelDiv: HTMLDivElement;
    private selectedFilterLabelId: number = null;
    private selectedFilterName: string = null;
    private pagination: Pagination;
    private paginationOptions: Pagination.Options;
    private overrideSetQueryCallback: (text: string) => void;

    private localizationScope = "FavoriteJs";

    constructor(renderContainer: JQuery, inputTextbox: JQuery, bookType: BookTypeEnum, queryType: QueryTypeEnum) {
        this.inputTextbox = inputTextbox;
        this.renderContainer = renderContainer;
        this.queryType = queryType;
        this.bookType = bookType;
        this.favoriteManager = new FavoriteManager();
        this.favoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
        this.insertDialog = new InsertQueryDialog();
        this.isCreated = false;
        this.renderContainer.hide();
    }

    public make() {
        if (this.isCreated) {
            return;
        }

        this.forceRerender();
        this.favoriteDialog.make();
        this.insertDialog.make();
    }

    public setOverrideQueryCallback(callback: (text: string) => void) {
        this.overrideSetQueryCallback = callback;
    }

    private forceRerender() {
        this.selectedFilterLabelId = null;
        this.selectedFilterName = null;

        this.renderLoading();
        
        this.favoriteManager.getFavoriteLabels(favoriteLabels => {
            this.render(favoriteLabels);
            this.bindEvents();
            this.isCreated = true;

            this.loadQueries();
        });
    }

    private loadQueries(defaultPageNumber = 1) {
        this.renderLoadingQueries();
        this.paginationOptions.callPageClickCallbackOnInit = false;
        this.pagination.make(0, FavoriteQuery.pageSize);
        this.favoriteManager.getFavoriteQueriesCount(this.selectedFilterLabelId, this.bookType, this.queryType, this.selectedFilterName, count => {
            this.paginationOptions.callPageClickCallbackOnInit = true;
            this.pagination.make(count, FavoriteQuery.pageSize, defaultPageNumber);
        });
    }

    private loadQueriesPage(pageNumber: number) {
        var count = FavoriteQuery.pageSize;
        var start = (pageNumber - 1) * count;
        this.favoriteManager.getFavoriteQueries(this.selectedFilterLabelId, this.bookType, this.queryType, this.selectedFilterName, start, count, favoriteQueries => {
            this.renderFavoriteQueries(favoriteQueries);
            this.bindLabelEvents();
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

    private renderLoadingQueries() {
        var loadingDiv = document.createElement("div");
        $(loadingDiv).addClass("loading");

        $(this.listContainer)
            .empty()
            .append(loadingDiv);
    }

    private render(favoriteLabels: IFavoriteLabel[]) {
        var mainDiv = document.createElement("div");
        var row1Div = document.createElement("div");
        var filterColumnDiv = document.createElement("div");
        var filterHeading = document.createElement("div");
        var filterHeaderSpan = document.createElement("span");
        var filterInputContainer = document.createElement("div");
        var filterInput = document.createElement("input");
        var filterSeparator = document.createElement("hr");
        var filterContainer = document.createElement("div");
        var noFilteredLabel = document.createElement("div");
        var displayAllLink = document.createElement("a");
        
        var listColumnDiv = document.createElement("div");
        var listHeading = document.createElement("div");
        var listHeaderContainer = document.createElement("div");
        var listHeaderSpan = document.createElement("span");
        var listHeaderLabel = document.createElement("span");
        var listHeaderFilterContainer = document.createElement("div");
        var listSeparator = document.createElement("hr");
        var listContainer = document.createElement("div");

        $(filterHeaderSpan)
            //.addClass("col-md-5")
            .addClass("favorite-query-header")
            .addClass("favorite-query-header-label")
            .text(localization.translate("Filter", this.localizationScope).value);
        $(filterInput)
            .attr("type", "text")
            .attr("placeholder", localization.translate("TagName", this.localizationScope).value)
            .attr("title", localization.translate("FilterTagsByName", this.localizationScope).value)
            .addClass("form-control")
            .addClass("input-sm");
        $(filterInputContainer)
            //.addClass("col-md-7")
            .addClass("favorite-query-filter-container")
            .append(filterInput);
        this.filterLabelInput = filterInput;

        $(filterHeading)
            //.addClass("row")
            .append(filterHeaderSpan)
            .append(filterInputContainer);

        $(displayAllLink)
            .attr("href", "#")
            .addClass("favorite-query-label")
            .data("id", 0)
            .data("name", localization.translate("AllShowed", this.localizationScope).value)
            .data("color", "#0000DD")
            .text("Zobrazit vše");
        this.displayAllLink = displayAllLink;

        $(noFilteredLabel)
            .addClass("text-center")
            .text(localization.translate("NoTagsInFilter", this.localizationScope).value)
            .hide();
        this.noFilteredLabel = noFilteredLabel;

        $(filterContainer)
            .addClass("favorite-query-list")
            .addClass("favorite-query-list-left");
        this.labelContainer = filterContainer;

        $(filterColumnDiv)
            .addClass("col-md-3")
            .append(filterHeading)
            .append(filterSeparator)
            .append(filterContainer);

        this.renderFavoriteLabels(favoriteLabels);

        $(listHeaderSpan)
            .text(localization.translate("InsertQueryFromFav", this.localizationScope).value);
        $(listHeaderLabel)
            .addClass("label")
            .addClass("favorite-query-label-selected")
            .css("background-color", "#0000DD")
            .text(localization.translate("AllShowed", this.localizationScope).value);

        $(listHeaderContainer)
            .addClass("col-md-8")
            .addClass("favorite-query-header")
            .append(listHeaderSpan)
            .append(listHeaderLabel);

        this.filterQuerySearchBox = new FilterSearchBox();
        this.filterQuerySearchBox.make();
        
        $(listHeaderFilterContainer)
            .addClass("col-md-4")
            .append(this.filterQuerySearchBox.getRootElement());

        $(listHeading)
            .addClass("row")
            .append(listHeaderContainer)
            .append(listHeaderFilterContainer);

        $(listContainer)
            .addClass("favorite-query-list");
        this.listContainer = listContainer;

        $(listColumnDiv)
            .addClass("col-md-9")
            .append(listHeading)
            .append(listSeparator)
            .append(listContainer);

        var noQueryDiv = document.createElement("div");
        $(noQueryDiv)
            .css("margin-left", "15px")
            .text(localization.translate("NoQueryInFilter", this.localizationScope).value)
            .hide();
        var noSelectedLabelDiv = document.createElement("div");
        $(noSelectedLabelDiv)
            .css("margin-left", "15px")
            .text(localization.translate("ChooseTag", this.localizationScope).value)
            .hide();
        this.noQueryDiv = noQueryDiv;
        this.noSelectedLabelDiv = noSelectedLabelDiv;

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
        var row3ButtonDiv = document.createElement("div");
        var row3Pagination = document.createElement("div");
        var saveButton = document.createElement("button");
        var buttonIcon = document.createElement("span");
        var buttonText = document.createElement("span");

        $(row3Pagination)
            .addClass("col-md-offset-3")
            .addClass("col-md-5")
            .addClass("bottom-pagination")
            .addClass("favorite-queries-pagination");

        $(buttonIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-star-empty");
        $(buttonText)
            .text(localization.translate("SaveCurrentQuery", this.localizationScope).value);

        $(saveButton)
            .addClass("btn")
            .addClass("btn-default")
            .addClass("btn-block")
            .addClass("favorite-query-save-button")
            .append(buttonIcon)
            .append(buttonText);

        $(row3ButtonDiv)
            .addClass("col-md-4")
            .append(saveButton);
        
        $(row3Div)
            .addClass("row")
            .append(row3Pagination)
            .append(row3ButtonDiv);

        $(mainDiv)
            .addClass("favorite-query")
            .append(row1Div)
            .append(row2Div)
            .append(row3Div);

        this.renderContainer.empty();
        this.renderContainer.append(mainDiv);

        this.paginationOptions = {
            container: $(".favorite-queries-pagination"),
            maxVisibleElements: 7,
            pageClickCallback: this.loadQueriesPage.bind(this)
        }
        this.pagination = new Pagination(this.paginationOptions);
    }

    private renderFavoriteLabels(favoriteLabels: IFavoriteLabel[]) {
        $(this.labelContainer)
            .empty()
            .append(this.displayAllLink)
            .append(this.noFilteredLabel);

        for (let i = 0; i < favoriteLabels.length; i++) {
            var favoriteLabel = favoriteLabels[i];
            var labelLink = document.createElement("a");
            var label = document.createElement("span");

            let color = new HexColor(favoriteLabel.color);
            let fontColor = FavoriteHelper.getDefaultFontColor(color);
            let borderColor = FavoriteHelper.getDefaultBorderColor(color);

            $(label)
                .addClass("label")
                .css("background-color", favoriteLabel.color)
                .css("border-color", borderColor)
                .css("color", fontColor)
                .text(favoriteLabel.name);

            $(labelLink)
                .attr("href", "#")
                .addClass("favorite-query-label")
                .data("id", favoriteLabel.id)
                .data("name", favoriteLabel.name)
                .data("color", favoriteLabel.color)
                .append(label);

            $(this.labelContainer).append(labelLink);
        }
    }

    private renderFavoriteQueries(favoriteQueries: IFavoriteQuery[]) {
        $(this.listContainer)
            .empty()
            .append(this.noQueryDiv)
            .append(this.noSelectedLabelDiv);
        $([this.noQueryDiv, this.noSelectedLabelDiv]).hide();

        for (let i = 0; i < favoriteQueries.length; i++) {
            var favoriteQuery = favoriteQueries[i];
            var queryLink = document.createElement("a");
            var queryRow1 = document.createElement("div");
            var queryRow2 = document.createElement("div");
            var queryLabel = document.createElement("span");
            var queryTitle = document.createElement("span");
            var queryRemoveLink = document.createElement("a");
            var queryRemoveIcon = document.createElement("span");
            var querySeparator = document.createElement("hr");

            $(queryRemoveIcon)
                .addClass("glyphicon")
                .addClass("glyphicon-trash");

            $(queryRemoveLink)
                .attr("href", "#")
                .addClass("favorite-query-remove")
                .data("id", favoriteQuery.id)
                .append(queryRemoveIcon);

            let color = new HexColor(favoriteQuery.favoriteLabel.color);
            let fontColor = FavoriteHelper.getDefaultFontColor(color);
            let borderColor = FavoriteHelper.getDefaultBorderColor(color);

            $(queryLabel)
                .addClass("label")
                .css("background-color", favoriteQuery.favoriteLabel.color)
                .css("border-color", borderColor)
                .css("color", fontColor)
                .text(favoriteQuery.favoriteLabel.name);

            $(queryTitle)
                .text(" " + favoriteQuery.title);

            $(queryRow1)
                .addClass("favorite-query-item-header")
                .append(queryLabel)
                .append(queryTitle);

            $(queryRow2)
                .addClass("favorite-query-raw")
                .text(favoriteQuery.query);

            $(queryLink)
                .attr("href", "#")
                .addClass("favorite-query-item")
                .data("id", favoriteQuery.id)
                .data("name", favoriteQuery.title)
                .data("query", favoriteQuery.query)
                .data("label-id", favoriteQuery.favoriteLabel.id)
                .append(queryRemoveLink)
                .append(queryRow1)
                .append(queryRow2);

            $(this.listContainer).append(queryLink).append(querySeparator);
        }
        if (favoriteQueries.length === 0) {
            $(this.noQueryDiv).show();
        }
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
            var color = new HexColor(labelColor);
            var fontColor = FavoriteHelper.getDefaultFontColor(color);
            var borderColor = FavoriteHelper.getDefaultBorderColor(color);

            $(".favorite-query-label-selected", this.renderContainer)
                .data("id", labelId)
                .text(labelName)
                .css("color", fontColor)
                .css("border-color", borderColor)
                .css("background-color", labelColor)
                .show();

            if (labelId === 0) {
                labelId = null;
                this.selectedFilterName = null;
                this.filterQuerySearchBox.clear();
                $(this.filterLabelInput).val("");
                this.filterLabels("");
            }

            this.selectedFilterLabelId = labelId;
            this.loadQueries();
        });

        $(".favorite-query-save-button", this.renderContainer).click(() => {
            this.favoriteDialog.show(localization.translate("NewFavQuery", this.localizationScope).value);
        });

        $(this.filterLabelInput).on("change keyup paste", () => {
            var value = String($(this.filterLabelInput).val());
            this.filterLabels(value);
        });

        this.filterQuerySearchBox.submit(() => {
            var value = this.filterQuerySearchBox.val();
            this.selectedFilterName = value.toLocaleLowerCase();
            this.loadQueries();
        });

        this.favoriteDialog.setSaveCallback(data => {
            var labelIds = new Array<number>();
            for (var i = 0; i < data.labels.length; i++) {
                var id = data.labels[i].labelId;
                labelIds.push(id);
            }
            this.saveFavoriteQuery(data.itemName, labelIds);
        });
    }

    private bindLabelEvents() {
        $(".favorite-query-item", this.renderContainer).click((event) => {
            var elementJquery = $(event.currentTarget);
            var query = elementJquery.data("query");

            this.insertQueryToSearchField(query);
        });

        $(".favorite-query-remove", this.renderContainer).click((event) => {
            event.stopPropagation();

            var elementJQuery = $(event.currentTarget);
            var favoriteId = elementJQuery.data("id");
            this.favoriteManager.deleteFavoriteItem(favoriteId, () => {
                //elementJQuery.closest(".favorite-query-item").remove();
                this.loadQueries(this.pagination.getCurrentPage());
            });
        });
    }

    private insertQueryToSearchField(newQuery: string) {
        var originalQuery = this.inputTextbox.val();
        
        if (originalQuery) {
            this.insertDialog.show(() => {
                this.directInsertQueryToSearchField(newQuery);
                this.insertDialog.hide();
            });
            return;
        }

        this.directInsertQueryToSearchField(newQuery);
    }

    private directInsertQueryToSearchField(query: string) {
        if (typeof this.overrideSetQueryCallback === "function") {
            this.overrideSetQueryCallback(query);
        } else {
            this.inputTextbox.val(query);
        }
    }

    private filterLabels(filter: string) {
        filter = filter.toLocaleLowerCase();
        var anyVisible = false;
        $(".favorite-query-label", this.renderContainer).each((index, element) => {
            if (index === 0) return;

            var labelName = String($(element).data("name")).toLocaleLowerCase();
            if (labelName.indexOf(filter) !== -1) {
                $(element).show();
                anyVisible = true;
            } else {
                $(element).hide();
            }
        });

        if (anyVisible) {
            $(this.noFilteredLabel).hide();
        } else {
            $(this.noFilteredLabel).show();
        }

        $(this.noQueryDiv).hide();
        $(this.noSelectedLabelDiv).show();
        $(".favorite-query-item", this.renderContainer).addClass("hidden");
        $(".favorite-query-list hr", this.renderContainer).addClass("hidden");
        $(".favorite-query-label-selected", this.renderContainer).hide();

        if (filter.length > 0) {
            this.paginationOptions.callPageClickCallbackOnInit = false;
            this.pagination.make(0, FavoriteQuery.pageSize);
        }
    }
    
    private saveFavoriteQuery(itemName: string, labelIds: number[]) {
        var query = this.inputTextbox.val();
        this.favoriteManager.createFavoriteQuery(this.bookType, this.queryType, query, itemName, labelIds, (id, error) => {
            if (error) {
                this.favoriteDialog.showError(localization.translate("CreateFavQueryError", this.localizationScope).value);
                return;
            }

            this.forceRerender();
            this.favoriteDialog.hide();
        });
    }
}

class InsertQueryDialog {
    private container: HTMLDivElement;
    private submitCallback: () => void;

    private localizationScope = "FavoriteJs";


    public make() {
        this.container = document.createElement("div");
        var dialog = document.createElement("div");
        var content = document.createElement("div");
        var header = document.createElement("div");
        var body = document.createElement("div");
        var footer = document.createElement("div");
        var closeButton = document.createElement("button");
        var title = document.createElement("h4");
        var noButton = document.createElement("button");
        var yesButton = document.createElement("button");

        $(title)
            .addClass("modal-title")
            .text(localization.translate("InsertQuery", this.localizationScope).value);
        $(closeButton)
            .attr("type", "button")
            .attr("data-dismiss", "modal")
            .addClass("close")
            .html("&times;");

        $(body)
            .addClass("modal-body")
            .text(localization.translate("ReplaceQuery", this.localizationScope).value);

        $(noButton)
            .attr("type", "button")
            .attr("data-dismiss", "modal")
            .addClass("btn")
            .addClass("btn-default")
            .text(localization.translate("Close", this.localizationScope).value);

        $(yesButton)
            .attr("type", "button")
            .addClass("btn")
            .addClass("btn-default")
            .text(localization.translate("Insert", this.localizationScope).value)
            .click(this.onSubmitClick.bind(this));

        $(header)
            .addClass("modal-header")
            .append(closeButton)
            .append(title);

        $(footer)
            .addClass("modal-footer")
            .append(noButton)
            .append(yesButton);

        $(content)
            .addClass("modal-content")
            .append(header)
            .append(body)
            .append(footer);

        $(dialog)
            .addClass("modal-dialog")
            .append(content);
        $(this.container)
            .addClass("modal")
            .addClass("fade")
            .attr("role", "dialog")
            .append(dialog);

        $("body").append(this.container);
    }

    public show(submitCallback: () => void) {
        this.submitCallback = submitCallback;
        $(this.container).modal("show");
    }

    public hide() {
        $(this.container).modal("hide");
    }

    private onSubmitClick() {
        if (this.submitCallback) {
            this.submitCallback();
        }
    }
}

class FilterSearchBox{
    private groupContainer: HTMLDivElement;
    private input: HTMLInputElement;
    private searchButton: HTMLButtonElement;

    private localizationScope = "FavoriteJs";

    public make() {
        this.groupContainer = document.createElement("div");
        var input = document.createElement("input");
        var buttonGroup = document.createElement("span");
        var searchButton = document.createElement("button");
        var searchIcon = document.createElement("span");

        $(searchIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-search");
        $(searchButton)
            .attr("type", "button")
            .addClass("btn")
            .addClass("btn-default")
            .append(searchIcon);
        this.searchButton = searchButton;

        $(buttonGroup)
            .addClass("input-group-btn")
            .append(searchButton);

        $(input)
            .attr("type", "text")
            .addClass("form-control")
            .attr("placeholder", localization.translate("SearchQuery", this.localizationScope).value)
            .attr("title", localization.translate("SearchByName", this.localizationScope).value);
        this.input = input;

        $(this.groupContainer)
            .addClass("input-group")
            .addClass("input-group-sm")
            .append(input)
            .append(buttonGroup);
    }

    public getRootElement(): HTMLDivElement {
        return this.groupContainer;
    }

    public change(handler: (eventObject: JQueryEventObject) => any) {
        return $(this.input).change(handler);
    }

    public submit(handler: (eventObject: JQueryEventObject) => any) {
        $(this.input).keypress(event => {
            if (event.keyCode === 13) {
                handler(null);
            }
        });
        return $(this.searchButton).click(handler);
    }

    public val(): string {
        return $(this.input).val();
    }

    public clear() {
        $(this.input).val("");
    }
}