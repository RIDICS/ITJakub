function initReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string, initPageXmlId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    var readerPanels = [ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel, ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel];
    var panelButtons = [PanelButtonEnum.Close, PanelButtonEnum.Pin, PanelButtonEnum.ToNewWindow];

    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0], readerPageChangedCallback, readerPanels, panelButtons, panelButtons);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search: Search;

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

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.searchPanelRemoveLoading();
                readerPlugin.showSearchInPanel(convertedResults);
            }
        });
    }

    function editionBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.searchPanelRemoveLoading();
                readerPlugin.showSearchInPanel(convertedResults);
            }
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

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookCount",
            data: { text: text, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId()  },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", text);
                readerPlugin.setResultsPaging(response["count"], paginatorPageClickCallback);
            }
        });

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPagesWithMatchHit",
            data: { text: text, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                readerPlugin.showSearchResultInPages(text, false, response["pages"]);
            }
        });
    }

    function advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookCount",
            data: { json: json, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId()  },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", json);
                readerPlugin.setResultsPaging(response["count"], paginatorPageClickCallback);
            }
        });

        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPagesWithMatchHit",
            data: { json: json, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                readerPlugin.showSearchResultInPages(json, true, response["pages"]);
            }
        });
    }


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

    if (typeof initPageXmlId !== "undefined" && initPageXmlId !== null) {
        var decodedText = decodeURIComponent(initPageXmlId);
        decodedText = replaceSpecialChars(decodedText);
        var pageId = Number(decodedText);
        readerPlugin.moveToPage(pageId, true);
    }

    //label item in main menu
    $('#main-plugins-menu').find('li').removeClass('active');
    var mainMenuLi = $('#editions-menu');
    $(mainMenuLi).addClass('active');
}

function listBookReadClickedOld(target) {
    return context => {
        var bookId = $(target).parents("li.list-item").attr("data-id");
        if (context.search.isLastQueryJson()) { //only text seach criteria we should propagate
            return onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId + "&searchText=" + context.search.getLastQuery());
        } else {
            return onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId);
        }
    };
}

function searchBookReadClickedOld(target) {
    return context => {
        var bookId = $(target).parents("li.list-item").attr("data-id");
        return onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId + "&searchText=" + context.search.getLastQuery());
    };
}