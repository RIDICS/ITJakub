$(document).ready(() => {
    var bookCountPerPage = 20;
    var dictionariesList = new DictionariesList(bookCountPerPage);
    dictionariesList.create();
});

class DictionariesList {
    private bookCountPerPage: number;
    private search: Search;
    private typeaheadSearchBox: SearchBox;
    private dictionarySelector: DropDownSelect2;
    private bibliographyModule: BibliographyModule;
    private bookIds: Array<number>;
    private categoryIds: Array<number>;

    constructor(bookCountPerPage: number) {
        this.bookCountPerPage = bookCountPerPage;
    }

    create() {
        var disabledOptions = new Array<SearchTypeEnum>();
        disabledOptions.push(SearchTypeEnum.Fulltext);
        disabledOptions.push(SearchTypeEnum.TokenDistance);
        disabledOptions.push(SearchTypeEnum.Sentence);
        disabledOptions.push(SearchTypeEnum.Heading);
        disabledOptions.push(SearchTypeEnum.Headword);
        disabledOptions.push(SearchTypeEnum.HeadwordDescription);
        disabledOptions.push(SearchTypeEnum.HeadwordDescriptionTokenDistance);

        this.search = new Search(<any>$("#listSearchDiv")[0], this.advancedSearch.bind(this), this.basicSearch.bind(this));
        this.search.makeSearch(disabledOptions);

        this.typeaheadSearchBox = new SearchBox(".searchbar-input", "Dictionaries/Dictionaries");
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
        };
        callbackDelegate.dataLoadedCallback = () => {
            $("#listResults").removeClass("loader");
            this.search.processSearchQuery("%"); //search for all by default criteria (title)
            this.search.writeTextToTextField("");
        }

        this.dictionarySelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
        this.dictionarySelector.makeDropdown();

        this.bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", this.sortOrderChanged.bind(this), BookTypeEnum.Dictionary);


        $(".searchbar-input.tt-input").change(() => {        //prevent clearing input value on blur() 
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
        });
    }

    private advancedSearch(jsonData: string) {
        var selectedIds = this.dictionarySelector.getSelectedIds();
        this.bookIds = selectedIds.selectedBookIds;
        this.categoryIds = selectedIds.selectedCategoryIds;
        
        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryAdvancedSearchResultsCount",
            data: { json: jsonData, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, this.pageClickCallbackForBiblModule.bind(this), response["count"]); //enable pagination
            }
        });
    }

    private basicSearch(text: string) {
        var selectedIds = this.dictionarySelector.getSelectedIds();
        this.bookIds = selectedIds.selectedBookIds;
        this.categoryIds = selectedIds.selectedCategoryIds;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchResultsCount",
            data: { text: text, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, this.pageClickCallbackForBiblModule.bind(this), response["count"]); //enable pagination
            }
        });
    }

    private advancedSearchPaged(json: string, pageNumber: number) {
        if (!json) return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryAdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
            }
        });
    }

    private basicSearchPaged(text: string, pageNumber: number) {
        if (!text) return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/DictionaryBasicSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
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

    private sortOrderChanged() {
        this.search.processSearch();
    }
}