
function initList() {
    var bookCountPerPage = 5;
    var corpusList = new CorpusList(bookCountPerPage);
    corpusList.create();
}

class CorpusList {
    private bookCountPerPage: number;
    private search: Search;
    private typeaheadSearchBox: SearchBox;
    private edition: DropDownSelect2;
    private bibliographyModule: BibliographyModule;
    private bookIds: Array<number>;
    private categoryIds: Array<number>;

    constructor(bookCountPerPage: number) {
        this.bookCountPerPage = bookCountPerPage;
    }

    create() {
        var disabledOptions = new Array<SearchTypeEnum>();

        this.search = new Search(<any>$("#listSearchDiv")[0], (json: string) => { this.advancedSearch(json) }, (text: string) => { this.basicSearch(text) });
        this.search.makeSearch(disabledOptions);

        this.typeaheadSearchBox = new SearchBox(".searchbar-input", "BohemianTextBank/BohemianTextBank");
        this.typeaheadSearchBox.addDataSet("Title", "Název");
        this.typeaheadSearchBox.create();
        this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

        var callbackDelegate = new DropDownSelectCallbackDelegate();
        callbackDelegate.selectedChangedCallback = (state: State) => {
            var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
            this.typeaheadSearchBox.clearAndDestroy();
            this.typeaheadSearchBox.addDataSet("Title", "Název", parametersUrl);
            this.typeaheadSearchBox.create();
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

            var selectedIds = this.edition.getSelectedIds();
            this.bookIds = selectedIds.selectedBookIds;
            this.categoryIds = selectedIds.selectedCategoryIds;
        };
        callbackDelegate.dataLoadedCallback = () => {
            var selectedIds = this.edition.getSelectedIds();
            this.bookIds = selectedIds.selectedBookIds;
            this.categoryIds = selectedIds.selectedCategoryIds;
            this.search.processSearchQuery("%"); //search for all by default criteria (title)
            this.search.writeTextToTextField("");
        };
        this.edition = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories", true, callbackDelegate);
        this.edition.makeDropdown();

        this.bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", () => { this.sortOrderChanged() }, BookTypeEnum.TextBank);


        $(".searchbar-input.tt-input").change(() => { //prevent clearing input value on blur() 
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
        });
    }

    private advancedSearchPaged(json: string, pageNumber: number) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response["results"]);
            }
        });
    }

    private basicSearchPaged(text: string, pageNumber: number) {
        this.hideTypeahead();
        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response["results"]);
            }
        });
    }

    private pageClickCallbackForBiblModule(pageNumber: number) {

        if (this.search.isLastQueryJson()) {
            this.advancedSearchPaged(this.search.getLastQuery(), pageNumber);
        } else {
            this.basicSearchPaged(this.search.getLastQuery(), pageNumber);
        }
    }

    private basicSearch(text: string) {
        this.hideTypeahead();
        if (typeof text === "undefined" || text === null || text === "") return;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchCount",
            data: { text: text, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, response["count"]); //enable pagination
            }
        });
    }

    private advancedSearch(json: string) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, response["count"]); //enable pagination
            }
        });
    }

    private sortOrderChanged() {
        this.search.processSearch();
    }

    private hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    }

}

