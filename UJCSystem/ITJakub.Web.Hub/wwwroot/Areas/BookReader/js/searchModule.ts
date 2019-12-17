﻿class SearchModule {
    search: Search;
    sc: ServerCommunication;
    readerPlugin: ReaderLayout;
    bookId: string;
    versionId: string;
    searchType: string;

    constructor(searchContainer: HTMLDivElement, sc: ServerCommunication, readerPlugin: ReaderLayout, bookId: string, versionId: string, searchType: string) {
        this.sc = sc;
        this.readerPlugin = readerPlugin;
        this.bookId = bookId;
        this.versionId = versionId;
        var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.Edition,
            queryType: QueryTypeEnum.Reader
        };
        this.searchType = searchType;
        this.search = new Search(searchContainer, this.advancedSearch.bind(this), this.basicSearch.bind(this), favoriteQueriesConfig);
    }

    public initSearchModule(initPageId: string, searchedText: string) {
        var enabledOptions = new Array<SearchTypeEnum>();
        enabledOptions.push(SearchTypeEnum.Fulltext);
        enabledOptions.push(SearchTypeEnum.TokenDistance);
        enabledOptions.push(SearchTypeEnum.Sentence);
        enabledOptions.push(SearchTypeEnum.Heading);

        this.search.makeSearch(enabledOptions, false);

        var decodedText: string;
        if (typeof searchedText !== "undefined" && searchedText !== null) {
            decodedText = decodeURIComponent(searchedText);
            decodedText = replaceSpecialChars(decodedText);
            this.search.processSearchQuery(decodedText);
        }

        if (typeof initPageId !== "undefined" && initPageId !== null) {
            decodedText = decodeURIComponent(initPageId);
            decodedText = replaceSpecialChars(decodedText);
            var pageId = Number(decodedText);
            this.readerPlugin.bookHeader.moveToPage(pageId, true);
        }

        // don't label item in main menu
        //$('#main-plugins-menu').find('li').removeClass('active');
        //var mainMenuLi = $('#editions-menu');
        //$(mainMenuLi).addClass('active');

        this.readerPlugin.readerLayout.eventHub.on("fulltextSearchDone", (count: number, searchedText: string) => {
            this.search.setLastQuery(searchedText);
            updateQueryStringParameter("searchText", searchedText);
            this.readerPlugin.setResultsPaging(count, this.paginatorPageClickCallback.bind(this));
        })
    }

    private basicSearch(text: string) {
        if (typeof text === "undefined" || text === null || text === "") return;
        if (this.searchType == SearchType.Fulltext) {
            var textSearch: JQueryXHR = this.sc.textSearchBookCount(this.bookId, this.versionId, text);
            textSearch.done((response: { count: number }) => {
                this.readerPlugin.readerLayout.eventHub.emit("fulltextSearchDone", response.count, text);
            });

            var textSearchMatchHit: JQueryXHR = this.sc.textSearchMatchHit(this.bookId, this.versionId, text);
            textSearchMatchHit.done((response: { pages: Array<IPage> }) => {
                this.readerPlugin.readerLayout.eventHub.emit("showTextSearchMatch", text, false, response.pages);
            });
        } else if (this.searchType == SearchType.Terms) {
            var termsSearch: JQueryXHR = this.sc.textSearchOldGrammar(this.bookId, this.versionId, text);
            termsSearch.done((response: {results: PageDescription[]}) => {
                updateQueryStringParameter("searchText", text);
                this.readerPlugin.showSearchInTermsPanel(response.results);
            });
        }


    }

    private advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;
        if (this.searchType == SearchType.Fulltext) {
            var advancedSearch: JQueryXHR =
                this.sc.advancedSearchBookCount(this.bookId, this.versionId, json);
            advancedSearch.done((response: { count: number }) => {
                this.readerPlugin.readerLayout.eventHub.emit("fulltextSearchDone", response.count, json);
            });

            var advancedSearchMatchHit: JQueryXHR = this.sc.advancedSearchMatchHit(this.bookId, this.versionId, json);
            advancedSearchMatchHit.done((response: { pages: Array<IPage> }) => {
                this.readerPlugin.readerLayout.eventHub.emit("showTextSearchMatch", json, true, response.pages);
            });
        } else if (this.searchType == SearchType.Terms) {
            var termsSearch: JQueryXHR = this.sc.advancedSearchOldGrammar(this.bookId, this.versionId, json);
            termsSearch.done((response: {results: PageDescription[]}) => {
                updateQueryStringParameter("searchText", json);
                this.readerPlugin.showSearchInTermsPanel(response.results);
            });
        }


    }

    private paginatorPageClickCallback(pageNumber: number) {

        this.readerPlugin.searchPanelClearResults();
        this.readerPlugin.searchPanelShowLoading();

        if (this.search.isLastQueryJson()) {
            this.editionAdvancedSearchPaged(this.search.getLastQuery(), pageNumber);
        } else {
            this.editionBasicSearchPaged(this.search.getLastQuery(), pageNumber);
        }
    }

    private editionAdvancedSearchPaged(json: string, pageNumber: number) {
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * this.readerPlugin.getSearchResultsCountOnPage();
        var count = this.readerPlugin.getSearchResultsCountOnPage();

        var advancedSearch: JQueryXHR = this.sc.advancedSearchBookPaged(this.bookId, this.versionId, json, start, count);
        advancedSearch.done((response: { results: Array<IPageWithContext> }) => {

            var convertedResults = this.convertSearchResults(response.results);
            this.readerPlugin.searchPanelRemoveLoading();
            this.readerPlugin.showSearchInPanel(convertedResults);
        });
        advancedSearch.fail(() => {
            this.readerPlugin.getSearchResultPanel().getSearchResultDiv().innerHTML =
                localization.translate("searchResultFailed", "BookReader").value;
            this.readerPlugin.searchPanelRemoveLoading();
        });

    }

    private editionBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * this.readerPlugin.getSearchResultsCountOnPage();
        var count = this.readerPlugin.getSearchResultsCountOnPage();

        var textSearch: JQueryXHR = this.sc.textSearchBookPaged(this.bookId, this.versionId, text, start, count);
        textSearch.done((response: { results: Array<IPageWithContext> }) => {
            var convertedResults = this.convertSearchResults(response.results);
            this.readerPlugin.searchPanelRemoveLoading();
            this.readerPlugin.showSearchInPanel(convertedResults);
        });
        textSearch.fail(() => {
            this.readerPlugin.getSearchResultPanel().getSearchResultDiv().innerHTML =
                localization.translate("searchResultFailed", "BookReader").value;
            this.readerPlugin.searchPanelRemoveLoading();
        });

    }

    private convertSearchResults(responseResults: Array<Object>): SearchHitResult[] {
        var searchResults = new Array<SearchHitResult>();
        for (var i = 0; i < responseResults.length; i++) {
            var result = responseResults[i];
            var resultContextStructure = result["ContextStructure"];
            var searchResult = new SearchHitResult();
            searchResult.pageId = result["PageId"];
            searchResult.pageName = result["PageName"];
            searchResult.before = resultContextStructure["Before"];
            searchResult.match = resultContextStructure["Match"];
            searchResult.after = resultContextStructure["After"];
            searchResults.push(searchResult);
        }

        return searchResults;
    }

}

enum SearchType {
    Fulltext = "Fulltext",
    Terms = "Terms"
}