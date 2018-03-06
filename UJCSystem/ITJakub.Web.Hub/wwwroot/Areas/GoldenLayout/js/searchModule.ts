class SearchModule {
    search: Search;
    sc: ServerCommunication;
    readerPlugin: ReaderLayout;
    bookId: string;
    versionId: string

    constructor(searchContainer: HTMLDivElement, sc: ServerCommunication, readerPlugin: ReaderLayout, bookId: string, versionId: string) {
        this.sc = sc;
        this.readerPlugin = readerPlugin;
        this.bookId = bookId;
        this.versionId = versionId;
        var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.Edition,
            queryType: QueryTypeEnum.Reader
        };
        this.search = new Search(searchContainer, this.advancedSearch.bind(this), this.basicSearch.bind(this), favoriteQueriesConfig);
    }

    public initSearchModule(initPageId: string, searchedText: string) {
        var enabledOptions = new Array<SearchTypeEnum>();
        enabledOptions.push(SearchTypeEnum.Fulltext);
        enabledOptions.push(SearchTypeEnum.TokenDistance);
        enabledOptions.push(SearchTypeEnum.Sentence);
        enabledOptions.push(SearchTypeEnum.Heading);

        this.search.makeSearch(enabledOptions);

        if (typeof searchedText !== "undefined" && searchedText !== null) {
            var decodedText = decodeURIComponent(searchedText);
            decodedText = replaceSpecialChars(decodedText);
            this.search.processSearchQuery(decodedText);
        }

        if (typeof initPageId !== "undefined" && initPageId !== null) {
            var decodedText = decodeURIComponent(initPageId);
            decodedText = replaceSpecialChars(decodedText);
            var pageId = Number(decodedText);
            this.readerPlugin.moveToPage(pageId, true);
        }

        //label item in main menu
        $('#main-plugins-menu').find('li').removeClass('active');
        var mainMenuLi = $('#editions-menu');
        $(mainMenuLi).addClass('active');
    }

    private basicSearch(text: string) {
        if (typeof text === "undefined" || text === null || text === "") return;
        var textSearch: JQueryXHR = this.sc.textSearchBookCount(this.bookId, this.versionId, text);
        textSearch.done((response: { count: number }) => {
            updateQueryStringParameter("searchText", text);
            this.readerPlugin.setResultsPaging(response.count, this.paginatorPageClickCallback.bind(this));
        });

        var textSearchMatchHit: JQueryXHR = this.sc.textSearchMatchHit(this.bookId, this.versionId, text)
        textSearchMatchHit.done((response: { pages: Array<IPage> }) => {
            this.readerPlugin.showSearchResultInPages(text, false, response.pages);
        });
    }

    private advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;
        var advancedSearch: JQueryXHR =
            this.sc.advancedSearchBookCount(this.bookId, this.versionId, json);
        advancedSearch.done((response: { count: number }) => {
            updateQueryStringParameter("searchText", json);
            this.readerPlugin.setResultsPaging(response.count, this.paginatorPageClickCallback.bind(this));
        });

        var advancedSearchMatchHit: JQueryXHR = this.sc.advancedSearchMatchHit(this.bookId, this.versionId, json);
        advancedSearchMatchHit.done((response: { pages: Array<IPage> }) => {
            this.readerPlugin.showSearchResultInPages(json, true, response.pages);
        });

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