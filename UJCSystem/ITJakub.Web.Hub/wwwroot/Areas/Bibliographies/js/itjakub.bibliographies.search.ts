﻿

function initBiblSearch() {
    var bookCountPerPage = Number($("#bibliography-configuration").data("page-size"));
    var bibliographies = new BibliographiesSearch(bookCountPerPage);
    bibliographies.create();
}

class BibliographiesSearch {
    private bookCountPerPage: number;
    private search: Search;
    private typeaheadSearchBox: SearchBox;
    private bibliographyModule: BibliographyModule;
    private searchAreaSelector: SearchAreaSelectorWrapper;

    private localization: Localization;

    private urlSearchKey = "search";
    private urlPageKey = "page";
    private urlSortAscKey = "sortAsc";
    private urlSortCriteriaKey = "sortCriteria";
    
    private notInitialized = true;
    private initPage: number = null;

    private firstLoad: boolean = true;

    constructor(bookCountPerPage: number) {
        this.bookCountPerPage = bookCountPerPage;

        this.localization = localization;
    }

    create() {
        var enabledOptions = new Array<SearchTypeEnum>();
        enabledOptions.push(SearchTypeEnum.Title);
        enabledOptions.push(SearchTypeEnum.Author);
        enabledOptions.push(SearchTypeEnum.Editor);
        enabledOptions.push(SearchTypeEnum.Dating);

        var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.BibliographicalItem,
            queryType: QueryTypeEnum.Search
        };
        this.search = new Search(<any>$("#listSearchDiv")[0], (json: string) => { this.advancedSearch(json) }, (text: string) => { this.basicSearch(text) }, favoriteQueriesConfig);
        this.search.makeSearch(enabledOptions, false);
        this.search.setPlaceholder(localization.translate("SearchInTitles...", "PluginsJs").value);
        this.search.setOverrideQueryCallback(newQuery => this.typeaheadSearchBox.value(newQuery));

        this.typeaheadSearchBox = new SearchBox(".searchbar-input", "Bibliographies/Bibliographies");
        this.typeaheadSearchBox.addDataSet("Title", this.localization.translate("Titles", "Bibliographies").value);
        this.typeaheadSearchBox.addDataSet("Author", this.localization.translate("Authors", "Bibliographies").value);
        this.typeaheadSearchBox.create();
        this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

        this.bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", () => { this.sortOrderChanged() });

        this.searchAreaSelector = new SearchAreaSelectorWrapper($("#dropdown-search-area-selector"), () => { this.search.processSearch() });
        
        this.initializeFromUrlParams();
    }

    private initializeFromUrlParams() {

        if (this.notInitialized) {

            this.notInitialized = false;

            var page = getQueryStringParameterByName(this.urlPageKey);

            if (page) {
                this.initPage = parseInt(page);
            }

            var sortedAsc = getQueryStringParameterByName(this.urlSortAscKey);
            var sortCriteria = getQueryStringParameterByName(this.urlSortCriteriaKey);

            if (sortedAsc && sortCriteria) {
                this.bibliographyModule.setSortedAsc(sortedAsc === "true");
                this.bibliographyModule.setSortCriteria(<SortEnum>(<any>(sortCriteria)));
            }

            var searched = getQueryStringParameterByName(this.urlSearchKey);
            this.search.writeTextToTextField(searched);

            this.search.processSearch();
            

        } 
    }

    private advancedSearchPaged(json: string, pageNumber: number) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();
        var searchAreas = this.searchAreaSelector.getValues();

        if (!this.firstLoad) {
            this.bibliographyModule.clearBooks();
            this.bibliographyModule.showLoading();
        }
        this.firstLoad = false;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Bibliographies/Bibliographies/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, searchArea: searchAreas } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response["results"]);
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private basicSearchPaged(text: string, pageNumber: number) {
        this.hideTypeahead();
        //if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();
        var searchAreas = this.searchAreaSelector.getValues();

        if (!this.firstLoad) {
            this.bibliographyModule.clearBooks();
            this.bibliographyModule.showLoading();
        }
        this.firstLoad = false;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Bibliographies/Bibliographies/TextSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, searchArea: searchAreas } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response["results"]);
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
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
        //if (typeof text === "undefined" || text === null || text === "") return;

        var searchAreas = this.searchAreaSelector.getValues();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Bibliographies/Bibliographies/TextSearchCount",
            data: { text: text, searchArea: searchAreas } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private advancedSearch(json: string) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        var searchAreas = this.searchAreaSelector.getValues();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Bibliographies/Bibliographies/AdvancedSearchResultsCount",
            data: { json: json, searchArea: searchAreas } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private sortOrderChanged() {
        this.bibliographyModule.showPage(1);
    }

    private hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    }

    private createPagination(booksCount: number) {
        var pages = Math.ceil(booksCount / this.bookCountPerPage);
        if (this.initPage && this.initPage <= pages) {
            this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, booksCount, this.initPage);
        } else {
            this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, booksCount);
        }
    }

}
                