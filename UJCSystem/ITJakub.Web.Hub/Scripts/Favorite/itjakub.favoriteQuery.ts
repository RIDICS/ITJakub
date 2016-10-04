class FavoriteQuery {
    private queryType: QueryTypeEnum;
    private bookType: BookTypeEnum;
    private inputTextbox: JQuery;
    private renderContainer: JQuery;
    private favoriteManager: FavoriteManager;
    private favoriteDialog: NewFavoriteDialog;
    private isCreated: boolean;
    private noQueryDiv: HTMLDivElement;
    private insertDialog: InsertQueryDialog;
    private filterQuerySearchBox: FilterSearchBox;
    private filterLabelInput: HTMLInputElement;
    private noFilteredLabel: HTMLDivElement;
    private noSelectedLabelDiv: HTMLDivElement;
    private selectedFilterLabelId: number;
    private selectedFilterName: string;

    constructor(renderContainer: JQuery, inputTextbox: JQuery, bookType: BookTypeEnum, queryType: QueryTypeEnum) {
        this.queryType = queryType;
        this.bookType = bookType;
        this.favoriteManager = new FavoriteManager(StorageManager.getInstance().getStorage());
        this.favoriteDialog = new NewFavoriteDialog(this.favoriteManager, true);
        this.inputTextbox = inputTextbox;
        this.renderContainer = renderContainer;
        this.isCreated = false;
        this.insertDialog = new InsertQueryDialog();
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
            .addClass("col-md-5")
            .addClass("favorite-query-header")
            .text("Filtrovat:");
        $(filterInput)
            .attr("type", "text")
            .attr("placeholder", "Filtrovat")
            .attr("title", "Filtrovat štítky podle názvu")
            .addClass("form-control")
            .addClass("input-sm");
        $(filterInputContainer)
            .addClass("col-md-7")
            .append(filterInput);
        this.filterLabelInput = filterInput;

        $(filterHeading)
            .addClass("row")
            .append(filterHeaderSpan)
            .append(filterInputContainer);

        $(displayAllLink)
            .attr("href", "#")
            .addClass("favorite-query-label")
            .data("id", 0)
            .data("name", "Zobrazeno vše")
            .data("color", "#0000DD")
            .text("Zobrazit vše");

        $(noFilteredLabel)
            .addClass("text-center")
            .text("Žádný štítek odpovídající zadanému filtru")
            .hide();
        this.noFilteredLabel = noFilteredLabel;

        $(filterContainer)
            .addClass("favorite-query-list")
            .append(displayAllLink)
            .append(noFilteredLabel);

        $(filterColumnDiv)
            .addClass("col-md-3")
            .append(filterHeading)
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

        $(listHeaderSpan)
            .text("Vložit dotaz z oblíbených: ");
        $(listHeaderLabel)
            .addClass("label")
            .addClass("favorite-query-label-selected")
            .css("background-color", "#0000DD")
            .text("Zobrazeno vše");

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

        $(listColumnDiv)
            .addClass("col-md-9")
            .append(listHeading)
            .append(listSeparator)
            .append(listContainer);

        var noQueryDiv = document.createElement("div");
        $(noQueryDiv)
            .css("margin-left", "15px")
            .text("Žádný oblíbený dotaz odpovídající zvoleným filtrům")
            .hide();
        var noSelectedLabelDiv = document.createElement("div");
        $(noSelectedLabelDiv)
            .css("margin-left", "15px")
            .text("Pro zobrazení oblíbených dotazů vyberte štítek ze seznamu")
            .hide();
        $(listContainer)
            .append(noQueryDiv)
            .append(noSelectedLabelDiv);
        this.noQueryDiv = noQueryDiv;
        this.noSelectedLabelDiv = noSelectedLabelDiv;

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
                .data("name", favoriteQuery.Title)
                .data("query", favoriteQuery.Query)
                .data("label-id", favoriteQuery.FavoriteLabel.Id)
                .append(queryRemoveLink)
                .append(queryRow1)
                .append(queryRow2);

            $(listContainer).append(queryLink);
        }
        if (favoriteQueries.length === 0) {
            $(this.noQueryDiv).show();
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
            .text(" Uložit stávající dotaz");

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
                this.selectedFilterLabelId = 0;
                this.selectedFilterName = null;
                this.filterQuerySearchBox.clear();
                $(this.filterLabelInput).val("");
                this.filterLabels("");
            }

            this.selectedFilterLabelId = labelId;
            this.filterQueryList();
        });

        $(".favorite-query-item", this.renderContainer).click((event) => {
            var elementJquery = $(event.currentTarget);
            var query = elementJquery.data("query");

            this.insertQueryToSearchField(query);
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

        $(this.filterLabelInput).on("change keyup paste", () => {
            var value = String($(this.filterLabelInput).val());
            this.filterLabels(value);
        });

        this.filterQuerySearchBox.change(() => {
            var value = this.filterQuerySearchBox.val();
            this.selectedFilterName = value.toLocaleLowerCase();
            this.filterQueryList();
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

    private insertQueryToSearchField(newQuery: string) {
        var originalQuery = this.inputTextbox.val();
        
        if (originalQuery) {
            this.insertDialog.show(() => {
                this.inputTextbox.val(newQuery); 
                this.insertDialog.hide();
            });
            return;
        }

        this.inputTextbox.val(newQuery);
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
        $(".favorite-query-label-selected", this.renderContainer).hide();
    }

    private filterQueryList() {
        $(this.noSelectedLabelDiv).hide();

        var labelId = this.selectedFilterLabelId;
        var nameFilter = this.selectedFilterName;
        if (!labelId && !nameFilter) {
            $(".favorite-query-item").removeClass("hidden");
            if ($(".favorite-query-item").length > 0) {
                $(this.noQueryDiv).hide();
            } else {
                $(this.noQueryDiv).show();
            }
            return;
        }
        
        var isAnyVisible = false;
        $(".favorite-query-item")
            .addClass("hidden")
            .each((index, element) => {
                var elementLabelId = $(element).data("label-id");
                var queryName = String($(element).data("name")).toLocaleLowerCase();

                if (labelId && nameFilter) {
                    if (elementLabelId === labelId && queryName.indexOf(nameFilter) !== -1) {
                        $(element).removeClass("hidden");
                        isAnyVisible = true;
                    }
                } else if (labelId) {
                    if (elementLabelId === labelId) {
                        $(element).removeClass("hidden");
                        isAnyVisible = true;
                    }
                } else if (nameFilter) {
                    if (queryName.indexOf(nameFilter) !== -1) {
                        $(element).removeClass("hidden");
                        isAnyVisible = true;
                    }
                }
            });

        if (isAnyVisible) {
            $(this.noQueryDiv).hide();
        } else {
            $(this.noQueryDiv).show();
        }
    }

    private saveFavoriteQuery(itemName: string, labelIds: number[]) {
        var query = this.inputTextbox.val();
        this.favoriteManager.createFavoriteQuery(this.bookType, this.queryType, query, itemName, labelIds, (id, error) => {
            if (error) {
                this.favoriteDialog.showError("Chyba při vytváření oblíbeného dotazu");
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
            .text("Vložit dotaz");
        $(closeButton)
            .attr("type", "button")
            .attr("data-dismiss", "modal")
            .addClass("close")
            .html("&times;");

        $(body)
            .addClass("modal-body")
            .text("Opravdu chcete vložit zvolený dotaz a nahradit ním stávající?");

        $(noButton)
            .attr("type", "button")
            .attr("data-dismiss", "modal")
            .addClass("btn")
            .addClass("btn-default")
            .text("Zavřít");

        $(yesButton)
            .attr("type", "button")
            .addClass("btn")
            .addClass("btn-default")
            .text("Vložit")
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

        $(buttonGroup)
            .addClass("input-group-btn")
            .append(searchButton);

        $(input)
            .attr("type", "text")
            .addClass("form-control")
            .attr("placeholder", "Filtrovat")
            .attr("title", "Filtrovat podle názvu");
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

    public val(): string {
        return $(this.input).val();
    }

    public clear() {
        $(this.input).val("");
    }
}