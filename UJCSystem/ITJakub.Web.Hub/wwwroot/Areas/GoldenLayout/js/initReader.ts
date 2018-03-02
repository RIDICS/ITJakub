function initGoldenReader(bookId: string,
    versionId: string,
    bookTitle: string,
    pageList: any,
    searchedText?: string,
    initPageId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPanels = [
        ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel,
        ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel
    ];
    var sc = new ServerCommunication();
    var readerPlugin = new ReaderLayout(<any>$("#ReaderHeaderDiv")[0],
        readerPageChangedCallback,
        readerPanels,
        sc
    );
    readerPlugin.makeReader(bookId, versionId, bookTitle, pageList);
    var search: Search;
    var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
        bookType: BookTypeEnum.Edition,
        queryType: QueryTypeEnum.Reader
    };
    search = new Search(<any>$("#SearchDiv")[0], advancedSearch, basicSearch, favoriteQueriesConfig);
    var enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Fulltext);
    enabledOptions.push(SearchTypeEnum.TokenDistance);
    enabledOptions.push(SearchTypeEnum.Sentence);
    enabledOptions.push(SearchTypeEnum.Heading);

    search.makeSearch(enabledOptions);

    if (typeof searchedText !== "undefined" && searchedText !== null) {
        var decodedText = decodeURIComponent(searchedText);
        decodedText = replaceSpecialChars(decodedText);
        search.processSearchQuery(decodedText);
    }

    if (typeof initPageId !== "undefined" && initPageId !== null) {
        var decodedText = decodeURIComponent(initPageId);
        decodedText = replaceSpecialChars(decodedText);
        var pageId = Number(decodedText);
        readerPlugin.moveToPage(pageId, true);
    }

    //label item in main menu
    $('#main-plugins-menu').find('li').removeClass('active');
    var mainMenuLi = $('#editions-menu');
    $(mainMenuLi).addClass('active');

    function convertSearchResults(responseResults: Array<Object>): SearchHitResult[] {
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

    function editionAdvancedSearchPaged(json: string, pageNumber: number) {
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        var advancedSearch: JQueryXHR =
            sc.advancedSearchBookPaged(readerPlugin.getBookId(), readerPlugin.getVersionId(), json, start, count);
        advancedSearch.done((response: { results: Array<IPageWithContext> }) => {

            var convertedResults = convertSearchResults(response.results);
            readerPlugin.searchPanelRemoveLoading();
            readerPlugin.showSearchInPanel(convertedResults);
        });

    }

    function editionBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        var textSearch: JQueryXHR =
            sc.textSearchBookPaged(readerPlugin.getBookId(), readerPlugin.getVersionId(), text, start, count);
        textSearch.done((response: { results: Array<IPageWithContext> }) => {
            var convertedResults = convertSearchResults(response.results);
            readerPlugin.searchPanelRemoveLoading();
            readerPlugin.showSearchInPanel(convertedResults);
        });

    }

    function paginatorPageClickCallback(pageNumber: number) {

        readerPlugin.searchPanelClearResults();
        readerPlugin.searchPanelShowLoading();

        if (search.isLastQueryJson()) {
            editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function basicSearch(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;
        var textSearch: JQueryXHR = sc.textSearchBookCount(readerPlugin.getBookId(), readerPlugin.getVersionId(), text);
        textSearch.done((response: { count: number }) => {
            updateQueryStringParameter("searchText", text);
            readerPlugin.setResultsPaging(response.count, paginatorPageClickCallback);
        });

        var textSearchMatchHit: JQueryXHR =
            sc.textSearchMatchHit(readerPlugin.getBookId(), readerPlugin.getVersionId(), text)
        textSearchMatchHit.done((response: { pages: Array<IPage> }) => {
            readerPlugin.showSearchResultInPages(text, false, response.pages);
        });
    }

    function advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;
        var advancedSearch: JQueryXHR =
            sc.advancedSearchBookCount(readerPlugin.getBookId(), readerPlugin.getVersionId(), json);
        advancedSearch.done((response: { count: number }) => {
            updateQueryStringParameter("searchText", json);
            readerPlugin.setResultsPaging(response.count, paginatorPageClickCallback);
        });

        var advancedSearchMatchHit: JQueryXHR =
            sc.advancedSearchMatchHit(readerPlugin.getBookId(), readerPlugin.getVersionId(), json);
        advancedSearchMatchHit.done((response: { pages: Array<IPage> }) => {
            readerPlugin.showSearchResultInPages(json, true, response.pages);
        });

    }

    //var searchModule = new SearchModule(<any>$("#SearchDiv")[0], sc, readerPlugin, bookId, versionId);
    //searchModule.initSearchModule(initPageId,searchedText);
}